using Amethyst.Plugins.Contract;
using JoyConLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace plugin_JoyCon;

[Export(typeof(ITrackingDevice))]
[ExportMetadata("Name", "Joy-Con")]
[ExportMetadata("Guid", "K2VRTEAM-AME2-APII-DVCE-DVCENDJOYCON")]
[ExportMetadata("Publisher", "公彦赤屋先")]
[ExportMetadata("Version", "1.0.0.0")]
[ExportMetadata("Website", "https://github.com/KimihikoAkayasaki/plugin_JoyCon")]
public class JoyCon : ITrackingDevice
{
    [Import(typeof(IAmethystHost))] private IAmethystHost Host { get; set; }

    private bool PluginLoaded { get; set; }
    private JoyConManager Manager { get; set; } = new();

    public bool IsPositionFilterBlockingEnabled => false;
    public bool IsPhysicsOverrideEnabled => false;
    public bool IsSelfUpdateEnabled => false;
    public bool IsFlipSupported => false;
    public bool IsAppOrientationSupported => false;
    public bool IsSettingsDaemonSupported => false;
    public object SettingsInterfaceRoot => null;

    public bool IsInitialized => true;
    public bool IsSkeletonTracked { get; private set; }
    public int DeviceStatus { get; private set; } = 1;

    public ObservableCollection<TrackedJoint> TrackedJoints { get; } =
    [
        new() { Name = "⛓️‍💥 Joy-Con (L)", Role = TrackedJointType.JointManual },
        new() { Name = "⛓️‍💥 Joy-Con (R)", Role = TrackedJointType.JointManual }
    ];

    public string DeviceStatusString => PluginLoaded
        ? DeviceStatus switch
        {
            0 => Host.RequestLocalizedString("/Plugins/JCON/Statuses/Success"),
            1 => Host.RequestLocalizedString("/Plugins/JCON/Statuses/NotConnected"),
            _ => $"Undefined: {DeviceStatus}\nE_UNDEFINED\nSomething weird has happened, though we can't tell what."
        }
        : $"Undefined: {DeviceStatus}\nE_UNDEFINED\nSomething weird has happened, though we can't tell what.";

    public Uri ErrorDocsUri => new($"https://github.com/gb2111/JoyconLib-4-CS/issues");

    public void OnLoad()
    {
        PluginLoaded = true;
    }

    public void Initialize()
    {
        // Try connecting to the service
        try
        {
            // Initialize the API
            Manager.Scan();

            foreach (var j in Manager.J)
                j.ControllerDebugType = Joycon.DebugType.NONE;

            Manager.Start();

            // Rebuild controllers
            RefreshControllerList();

            // Refresh inside amethyst
            Host?.RefreshStatusInterface();
        }
        catch (Exception e)
        {
            Host?.Log($"Couldn't connect to the Service! {e.Message}");
        }
    }

    public void Shutdown()
    {
        // Try disconnection from the service
        try
        {
            lock (Host!.UpdateThreadLock)
            {
                Manager.OnApplicationQuit();
            }

            // Re-compute the status
            DeviceStatus = 1;

            // Refresh inside amethyst
            Host?.RefreshStatusInterface();
        }
        catch (Exception e)
        {
            Host?.Log($"Couldn't disconnect from the Service! {e.Message}");
        }
    }

    public void Update()
    {
        if (!PluginLoaded || DeviceStatus != 0) return; // Sanity check

        // Try connecting to the service
        try
        {
            Manager.Update(); // Update the service
            IsSkeletonTracked = false; // Stub check

            // Refresh on changes
            if (Manager.J.Count != TrackedJoints.Count) RefreshControllerList();

            // Refresh all controllers/all
            using var jointEnumerator = TrackedJoints.GetEnumerator();
            Manager.J.ForEach(controller =>
            {
                // Ove to the next controller list entry
                if (!jointEnumerator.MoveNext() ||
                    jointEnumerator.Current is null) return;

                // Note we're all fine
                IsSkeletonTracked = true;

                // Copy pose data from the controller
                jointEnumerator.Current.Position = Vector3.Zero;
                jointEnumerator.Current.Orientation = controller.Orientation;

                //// Copy physics data from the controller
                //jointEnumerator.Current.Velocity = controller.PoseVelocity();
                //jointEnumerator.Current.Acceleration = controller.PoseAcceleration();
                //jointEnumerator.Current.AngularVelocity = controller.PoseAngularVelocity();
                //jointEnumerator.Current.AngularAcceleration = controller.PoseAngularAcceleration();

                // Parse/copy the tracking state
                jointEnumerator.Current.TrackingState = TrackedJointState.StateTracked;

                // Update input actions (using extensions defined below)
                controller.UpdateActions(Host);
            });
        }
        catch (Exception e)
        {
            Host?.Log($"Couldn't update Service! {e.Message}");
            Host?.Log("Checking the service status again...");

            // Request a quick refresh of the status
            Host?.RefreshStatusInterface();
        }
    }

    public void SignalJoint(int jointId)
    {
        if (!PluginLoaded || DeviceStatus != 0) return; // Sanity check

        // Try buzzing the selected controller
        Task.Run(() =>
        {
            try
            {
                // Try setting controller rumble
                Manager.J.ElementAt(jointId).TryRumble();
            }
            catch (Exception e)
            {
                Host?.Log($"Couldn't re/set controller [{jointId}] rumble! {e.Message}");
            }
        });
    }

    private void RefreshControllerList()
    {
        // Try polling controllers and starting their streams
        try
        {
            Host?.Log("Locking the update thread...");
            lock (Host!.UpdateThreadLock)
            {
                Host?.Log("Emptying the tracked joints list...");
                TrackedJoints.Clear(); // Delete literally everything

                Host?.Log("Searching for tracked controllers...");
                if (Manager.J.Count >= 1) AddController("Joy-Con (L)");
                if (Manager.J.Count >= 2) AddController("Joy-Con (R)");

                // Re-compute the status
                DeviceStatus = 1;

                Host?.Log("Adding placeholders...");
                if (Manager.J.Count < 1)
                {
                    AddController("⛓️‍💥 Joy-Con (L)");
                    AddController("⛓️‍💥 Joy-Con (R)");

                    DeviceStatus = 0;
                }
            }

            // Refresh everything after the change
            Host?.Log("Refreshing the UI...");
            Host?.RefreshStatusInterface();
        }
        catch (Exception e)
        {
            Host?.Log($"Couldn't connect to the Service! {e.Message}");
        }

        return;

        void AddController(string name)
        {
            Host?.Log("Adding the new controller to the controller list...");
            TrackedJoints.Add(new TrackedJoint
            {
                Name = name,
                Role = TrackedJointType.JointManual,
                SupportedInputActions = DataExtensions.GetActions()
            });
        }
    }
}

public static class DataExtensions
{
    /*
        DPAD_DOWN = 0,
        DPAD_RIGHT = 1,
        DPAD_LEFT = 2,
        DPAD_UP = 3,
        SL = 4,
        SR = 5,
        MINUS = 6,
        HOME = 7,
        PLUS = 8,
        CAPTURE = 9,
        STICK = 10,
        SHOULDER_1 = 11,
        SHOULDER_2 = 12
     */

    public static void UpdateActions(this Joycon state, IAmethystHost host)
    {
        try
        {
            Enum.GetValues<Joycon.Button>()
                .Select((IKeyInputAction Action, object Data) (x) => (Action: new KeyInputAction<bool>
                {
                    Name = x.ToString(),
                    Guid = x.ToString()
                }, Data: state.GetButton(x))).ToList()
                .ForEach(x => host.ReceiveKeyInput(x.Action, x.Data));
        }
        catch (Exception e)
        {
            host?.Log(e);
        }
    }

    public static SortedSet<IKeyInputAction> GetActions()
    {
        return new SortedSet<IKeyInputAction>(
            Enum.GetValues<Joycon.Button>()
                .Select(IKeyInputAction (x) => new KeyInputAction<bool>
                {
                    Name = x.ToString(),
                    Guid = x.ToString()
                }).ToList());
    }
}