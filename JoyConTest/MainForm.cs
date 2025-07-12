using JoyConLib;

namespace JoyConTest;

public partial class MainForm : Form
{
    private readonly Math3D.Math3D.Cube cube1;
    private readonly Math3D.Math3D.Cube cube2;

    private readonly Point drawOrigin;

    private readonly JoyConManager _joyConManager = new();

    public MainForm()
    {
        InitializeComponent();

        drawOrigin = new Point(picture1.Width / 2, picture1.Height / 2);
        cube1 = new Math3D.Math3D.Cube(200, 100, 20);
        cube2 = new Math3D.Math3D.Cube(200, 100, 20);
    }


    private void buttonScan_Click(object sender, EventArgs e)
    {
        _joyConManager.Scan();

        UpdateDebugType();
        UpdateInfo();
    }

    private void UpdateDebugType()
    {
        foreach (var j in _joyConManager.J)
            j.ControllerDebugType = Joycon.DebugType.NONE;
    }


    private void buttonStart_Click(object sender, EventArgs e)
    {
        _joyConManager.Start();
        timerUpdate.Enabled = true;
    }

    private void timerUpdate_Tick(object sender, EventArgs e)
    {
        _joyConManager.Update();

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        if (_joyConManager.J.Count > 0)
        {
            var j = _joyConManager.J[0];

            label1.Text = j.ToString();

            cube1.InitializeCube();
            cube1.RotateX = (float)(j.GetVector().EulerAngles.Y * 180.0f / Math.PI);
            cube1.RotateY = (float)(j.GetVector().EulerAngles.Z * 180.0f / Math.PI);
            cube1.RotateZ = (float)(j.GetVector().EulerAngles.X * 180.0f / Math.PI);

            picture1.Image = cube1.DrawCube(drawOrigin);
        }
        else
        {
            if (label1.Text != "")
                label1.Text = "not found";
        }

        if (_joyConManager.J.Count > 1)
        {
            var j = _joyConManager.J[1];

            label2.Text = j.ToString();


            cube2.InitializeCube();
            cube2.RotateX = (float)(j.GetVector().EulerAngles.Y * 180.0f / Math.PI);
            cube2.RotateY = (float)(j.GetVector().EulerAngles.Z * 180.0f / Math.PI);
            cube2.RotateZ = (float)(j.GetVector().EulerAngles.X * 180.0f / Math.PI);

            picture2.Image = cube2.DrawCube(drawOrigin);
        }
        else
        {
            if (label2.Text != "")
                label2.Text = "not found";
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _joyConManager.OnApplicationQuit();
    }
}