using System.Runtime.InteropServices;

namespace JoyConLib;

public class JoyConManager
{
    // Different operating systems either do or don't like the trailing zero
    private const ushort VendorId = 0x57e;
    private const ushort VendorIdFallback = 0x057e;
    private const ushort ProductL = 0x2006;
    private const ushort ProductR = 0x2007;

    // Settings accessible via Unity
    public bool EnableImu = true;
    public bool EnableLocalize = true;

    public List<Joycon> J = []; // Array of all connected Joy-Cons

    public static JoyConManager Instance { get; private set; }

    public void Scan()
    {
        Instance ??= this;

        var isLeft = false;
        HidApi.hid_init();

        var ptr = HidApi.hid_enumerate(VendorId, 0x0);
        var topPtr = ptr;

        if (ptr == IntPtr.Zero)
        {
            ptr = HidApi.hid_enumerate(VendorIdFallback, 0x0);
            if (ptr == IntPtr.Zero)
            {
                HidApi.hid_free_enumeration(ptr);
                System.Diagnostics.Debug.WriteLine("No Joy-Cons found!");
            }
        }

        while (ptr != IntPtr.Zero)
        {
            var enumerate = (HidDeviceInfo)Marshal.PtrToStructure(ptr, typeof(HidDeviceInfo))!;
            var valid = false;

            System.Diagnostics.Debug.WriteLine(enumerate.ProductId);
            if (enumerate.ProductId is ProductL or ProductR)
            {
                switch (enumerate.ProductId)
                {
                    case ProductL:
                        valid = true;
                        isLeft = true;
                        System.Diagnostics.Debug.WriteLine("Left Joy-Con connected.");
                        break;
                    case ProductR:
                        valid = true;
                        isLeft = false;
                        System.Diagnostics.Debug.WriteLine("Right Joy-Con connected.");
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Non Joy-Con input device skipped.");
                        break;
                }

                if (valid)
                {
                    var handle = HidApi.hid_open_path(enumerate.Path);
                    HidApi.hid_set_nonblocking(handle, 1);
                    J.Add(new Joycon(handle, EnableImu, EnableLocalize & EnableImu, 0.04f, isLeft));
                }
            }

            ptr = enumerate.Next;
        }

        HidApi.hid_free_enumeration(topPtr);
    }

    public void Start()
    {
        for (var i = 0; i < J.Count; ++i)
        {
            System.Diagnostics.Debug.WriteLine(i);
            var jc = J[i];
            byte leDs = 0x0;
            leDs |= (byte)(0x1 << i);
            jc.Attach(leDs);
            jc.Begin();
        }
    }

    public void Update()
    {
        foreach (var jc in J) jc.Update();
    }

    public void OnApplicationQuit()
    {
        foreach (var jc in J) jc.Detach();
    }
}