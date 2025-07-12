#define DEBUG

using System.Diagnostics;

namespace JoyConLib;

public class Joycon
{
    public enum Button
    {
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
    }

    public enum DebugType
    {
        NONE = 0,
        ALL,
        COMMS,
        THREADING,
        IMU,
        RUMBLE,
        CUSTOM1
    }

    public enum State : uint
    {
        NOT_ATTACHED,
        DROPPED,
        NO_JOYCONS,
        ATTACHED,
        INPUT_MODE_0_X30,
        IMU_DATA_OK
    }

    private const uint ReportLen = 49;
    private readonly short[] _accR = [0, 0, 0];
    private readonly bool[] _buttons = new bool[13];
    private readonly bool[] _buttonsDown = new bool[13];
    private readonly bool[] _buttonsUp = new bool[13];

    private readonly byte[] _defaultBuf = [0x0, 0x1, 0x40, 0x40, 0x0, 0x1, 0x40, 0x40];

    private readonly bool _doLocalize;
    private readonly bool[] _down = new bool[13];
    private readonly short[] _gyrNeutral = [0, 0, 0];

    private readonly short[] _gyrR = [0, 0, 0];

    private readonly IntPtr _handle;
    private readonly bool _imuEnabled;
    private readonly float[] _max = [0, 0, 0];
    private readonly Queue<Report> _reports = new();
    private readonly ushort[] _stickCal = [0, 0, 0, 0, 0, 0];
    private readonly ushort[] _stickPrecal = [0, 0];

    private readonly byte[] _stickRaw = [0, 0, 0];
    private readonly float[] _sum = [0, 0, 0];
    private Vector3 _accG;
    private Vector3 _dTheta;
    private ushort _deadzone;
    private string _debugStr;
    public DebugType ControllerDebugType = DebugType.IMU;

    private float _err;
    private float _filterweight;
    private bool _firstImuPacket = true;

    private byte _globalCount;
    private Vector3 _gyrG;

    public Vector3 Ib, Jb, Kb, KAcc;
    private Vector3 _iB;
    public bool IsLeft;
    private Thread _pollThreadObj;
    internal bool RawUp;
    private Rumble _rumbleObj;
    public State ControllerState;

    private float[] _stick = [0, 0];

    private bool _stopPolling;
    private int _timestamp;
    private byte _tsDe;
    private byte _tsEn;
    private DateTime _tsPrev;
    private Quaternion _vec;
    private Vector3 _wA, _wG;

    public Joycon(IntPtr handle, bool imu, bool localize, float alpha, bool left)
    {
        _handle = handle;
        _imuEnabled = imu;
        _doLocalize = localize;
        _rumbleObj = new Rumble(160, 320, 0);
        _filterweight = alpha;
        IsLeft = left;
    }

    public void DebugPrint(string s, DebugType d)
    {
        if (ControllerDebugType == DebugType.NONE) return;
        if (d == DebugType.ALL || d == ControllerDebugType || ControllerDebugType == DebugType.ALL) Debug.WriteLine(s);
    }

    public bool GetButtonDown(Button b)
    {
        return _buttonsDown[(int)b];
    }

    public bool GetButton(Button b)
    {
        return _buttons[(int)b];
    }

    public bool GetButtonUp(Button b)
    {
        return _buttonsUp[(int)b];
    }

    public float[] GetStick()
    {
        return _stick;
    }

    public Vector3 GetGyro()
    {
        return _gyrG;
    }

    public Vector3 GetAccel()
    {
        return _accG;
    }

    public Quaternion GetVector()
    {
        return Vector3.LookRotation(new Vector3(Jb.X, Ib.X, Kb.X), -new Vector3(Jb.Z, Ib.Z, Kb.Z));
    }

    public System.Numerics.Quaternion Orientation
    {
        get
        {
            var q = GetVector();
            return new System.Numerics.Quaternion((float)q.X, (float)q.Y, (float)q.Z, (float)q.W);
        }
    }

    public int Attach(byte leds = 0x0)
    {
        ControllerState = State.ATTACHED;
        byte[] a = [0x0];
        // Input report mode
        Subcommand(0x3, [0x3f], 1, false);
        a[0] = 0x1;
        dump_calibration_data();
        // Connect
        a[0] = 0x01;
        Subcommand(0x1, a, 1);
        a[0] = 0x02;
        Subcommand(0x1, a, 1);
        a[0] = 0x03;
        Subcommand(0x1, a, 1);
        a[0] = leds;
        Subcommand(0x30, a, 1);
        Subcommand(0x40, [_imuEnabled ? (byte)0x1 : (byte)0x0], 1);
        Subcommand(0x3, [0x30], 1);
        Subcommand(0x48, [0x1], 1);
        DebugPrint("Done with init.", DebugType.COMMS);
        return 0;
    }

    public void SetFilterCoeff(float a)
    {
        _filterweight = a;
    }

    public void Detach()
    {
        _stopPolling = true;
        PrintArray(_max, format: "Max {0:S}", d: DebugType.IMU);
        PrintArray(_sum, format: "Sum {0:S}", d: DebugType.IMU);
        if (ControllerState > State.NO_JOYCONS)
        {
            Subcommand(0x30, [0x0], 1);
            Subcommand(0x40, [0x0], 1);
            Subcommand(0x48, [0x0], 1);
            Subcommand(0x3, [0x3f], 1);
        }

        if (ControllerState > State.DROPPED) HidApi.hid_close(_handle);
        ControllerState = State.NOT_ATTACHED;
    }

    private int ReceiveRaw()
    {
        if (_handle == IntPtr.Zero) return -2;
        HidApi.hid_set_nonblocking(_handle, 0);
        var rawBuf = new byte[ReportLen];
        var ret = HidApi.hid_read(_handle, rawBuf, new UIntPtr(ReportLen));
        if (ret > 0)
        {
            lock (_reports)
            {
                _reports.Enqueue(new Report(rawBuf, DateTime.Now));

                RawUp = (rawBuf[3 + (IsLeft ? 2 : 0)] & (IsLeft ? 0x02 : 0x02)) != 0;

                DebugPrint(string.Format("JoyCon {0} UP: {1}",
                    IsLeft ? "Left" : "Right", RawUp), DebugType.CUSTOM1);
            }

            if (_tsEn == rawBuf[1]) DebugPrint($"Duplicate timestamp enqueued. TS: {_tsEn:X2}", DebugType.THREADING);
            _tsEn = rawBuf[1];
            DebugPrint($"Enqueue. Bytes read: {ret:D}. Timestamp: {rawBuf[1]:X2}", DebugType.THREADING);
        }

        return ret;
    }

    private void Poll()
    {
        var attempts = 0;
        while (!_stopPolling & (ControllerState > State.NO_JOYCONS))
        {
            SendRumble(_rumbleObj.GetData());
            var a = ReceiveRaw();
            a = ReceiveRaw();

            if (a > 0)
            {
                ControllerState = State.IMU_DATA_OK;
                attempts = 0;
            }
            else if (attempts > 1000)
            {
                ControllerState = State.DROPPED;
                DebugPrint("Connection lost. Is the Joy-Con connected?", DebugType.ALL);
                break;
            }
            else
            {
                DebugPrint("Pause 5ms", DebugType.THREADING);
                Thread.Sleep(5);
            }

            ++attempts;
        }

        DebugPrint("End poll loop.", DebugType.THREADING);
    }

    public void Update()
    {
        if (ControllerState > State.NO_JOYCONS)
        {
            var reportBuf = new byte[ReportLen];
            while (_reports.Count > 0)
            {
                Report rep;
                lock (_reports)
                {
                    rep = _reports.Dequeue();
                    rep.CopyBuffer(reportBuf);
                }

                if (_imuEnabled)
                {
                    if (_doLocalize)
                        ProcessImu(reportBuf);
                    else
                        ExtractImuValues(reportBuf);
                }

                if (_tsDe == reportBuf[1]) DebugPrint($"Duplicate timestamp dequeued. TS: {_tsDe:X2}", DebugType.THREADING);
                _tsDe = reportBuf[1];
                //DebugPrint(string.Format("Dequeue. Queue length: {0:d}. Packet ID: {1:X2}. Timestamp: {2:X2}. Lag to dequeue: {3:s}. Lag between packets (expect 15ms): {4:s}",
                //  reports.Count, report_buf[0], report_buf[1], System.DateTime.Now.Subtract(rep.GetTime()), rep.GetTime().Subtract(ts_prev)), DebugType.THREADING);
                _tsPrev = rep.GetTime();
            }

            ProcessButtonsAndStick(reportBuf);
            if (!_rumbleObj.TimedRumble) return;
            if (_rumbleObj.T < 0)
                _rumbleObj.set_vals(160, 320, 0);
            // todo123 rumble_obj.t -= Time.deltaTime;
        }
    }

    private int ProcessButtonsAndStick(byte[] reportBuf)
    {
        if (reportBuf[0] == 0x00) return -1;

        _stickRaw[0] = reportBuf[6 + (IsLeft ? 0 : 3)];
        _stickRaw[1] = reportBuf[7 + (IsLeft ? 0 : 3)];
        _stickRaw[2] = reportBuf[8 + (IsLeft ? 0 : 3)];

        _stickPrecal[0] = (ushort)(_stickRaw[0] | ((_stickRaw[1] & 0xf) << 8));
        _stickPrecal[1] = (ushort)((_stickRaw[1] >> 4) | (_stickRaw[2] << 4));
        _stick = CenterSticks(_stickPrecal);
        lock (_buttons)
        {
            lock (_down)
            {
                for (var i = 0; i < _buttons.Length; ++i) _down[i] = _buttons[i];
            }

            _buttons[(int)Button.DPAD_DOWN] = (reportBuf[3 + (IsLeft ? 2 : 0)] & (IsLeft ? 0x01 : 0x04)) != 0;
            _buttons[(int)Button.DPAD_RIGHT] = (reportBuf[3 + (IsLeft ? 2 : 0)] & (IsLeft ? 0x04 : 0x08)) != 0;
            _buttons[(int)Button.DPAD_UP] = (reportBuf[3 + (IsLeft ? 2 : 0)] & (IsLeft ? 0x02 : 0x02)) != 0;
            _buttons[(int)Button.DPAD_LEFT] = (reportBuf[3 + (IsLeft ? 2 : 0)] & (IsLeft ? 0x08 : 0x01)) != 0;
            _buttons[(int)Button.HOME] = (reportBuf[4] & 0x10) != 0;
            _buttons[(int)Button.MINUS] = (reportBuf[4] & 0x01) != 0;
            _buttons[(int)Button.PLUS] = (reportBuf[4] & 0x02) != 0;
            _buttons[(int)Button.STICK] = (reportBuf[4] & (IsLeft ? 0x08 : 0x04)) != 0;
            _buttons[(int)Button.SHOULDER_1] = (reportBuf[3 + (IsLeft ? 2 : 0)] & 0x40) != 0;
            _buttons[(int)Button.SHOULDER_2] = (reportBuf[3 + (IsLeft ? 2 : 0)] & 0x80) != 0;
            _buttons[(int)Button.SR] = (reportBuf[3 + (IsLeft ? 2 : 0)] & 0x10) != 0;
            _buttons[(int)Button.SL] = (reportBuf[3 + (IsLeft ? 2 : 0)] & 0x20) != 0;
            lock (_buttonsUp)
            {
                lock (_buttonsDown)
                {
                    for (var i = 0; i < _buttons.Length; ++i)
                    {
                        _buttonsUp[i] = _down[i] & !_buttons[i];
                        _buttonsDown[i] = !_down[i] & _buttons[i];
                    }
                }
            }
        }

        return 0;
    }

    public override string ToString()
    {
        var str = "JoyCon " + (IsLeft ? "Left" : "Right") + Environment.NewLine;

        str += "Button Minus: " + GetButton(Button.MINUS) + Environment.NewLine;
        str += "Button Plus: " + GetButton(Button.PLUS) + Environment.NewLine;
        str += "Button Capture: " + GetButton(Button.CAPTURE) + Environment.NewLine;
        str += "Button Home: " + GetButton(Button.HOME) + Environment.NewLine;
        str += "Button ZR1: " + GetButton(Button.SHOULDER_1) + Environment.NewLine;
        str += "Button ZR2: " + GetButton(Button.SHOULDER_2) + Environment.NewLine;
        str += "Button Up: " + GetButton(Button.DPAD_UP) + Environment.NewLine;
        str += "Button Up (raw): " + RawUp + Environment.NewLine;

        str += "Button Left: " + GetButton(Button.DPAD_LEFT) + Environment.NewLine;
        str += "Button Right: " + GetButton(Button.DPAD_RIGHT) + Environment.NewLine;
        str += "Button Down: " + GetButton(Button.DPAD_DOWN) + Environment.NewLine;
        str += "Button SL: " + GetButton(Button.SL) + Environment.NewLine;
        str += "Button SR: " + GetButton(Button.SR) + Environment.NewLine;
        str += "Button Stick Press: " + GetButton(Button.STICK) + Environment.NewLine;
        str += "Stick Axis: " + GetStick()[0] + " - " + GetStick()[1] + Environment.NewLine;
        str += "Orientation: " + GetVector();
        return str;
    }

    private void ExtractImuValues(byte[] reportBuf, int n = 0)
    {
        _gyrR[0] = (short)(reportBuf[19 + n * 12] | ((reportBuf[20 + n * 12] << 8) & 0xff00));
        _gyrR[1] = (short)(reportBuf[21 + n * 12] | ((reportBuf[22 + n * 12] << 8) & 0xff00));
        _gyrR[2] = (short)(reportBuf[23 + n * 12] | ((reportBuf[24 + n * 12] << 8) & 0xff00));
        _accR[0] = (short)(reportBuf[13 + n * 12] | ((reportBuf[14 + n * 12] << 8) & 0xff00));
        _accR[1] = (short)(reportBuf[15 + n * 12] | ((reportBuf[16 + n * 12] << 8) & 0xff00));
        _accR[2] = (short)(reportBuf[17 + n * 12] | ((reportBuf[18 + n * 12] << 8) & 0xff00));
        for (var i = 0; i < 3; ++i)
        {
            //acc_g[i] = acc_r[i] * 0.00025;
            // gyr_g[i] = (gyr_r[i] - gyr_neutral[i]) * 0.00122187695f;
            // if (Math.Abs(acc_g[i]) > Math.Abs(max[i]))
            //	max[i] = acc_g[i];
            _accG.SetAxisValue(i, _accR[i] * 0.00025);
            _gyrG.SetAxisValue(i, (_gyrR[i] - _gyrNeutral[i]) * 0.00122187695f);
            if (Math.Abs(_accG.GetAxisValue(i)) > Math.Abs(_max[i]))
                _max[i] = (float)_accG.GetAxisValue(i);
        }
    }

    private int ProcessImu(byte[] reportBuf)
    {
        // Direction Cosine Matrix method
        // http://www.starlino.com/dcm_tutorial.html

        if (!_imuEnabled | (ControllerState < State.IMU_DATA_OK))
            return -1;

        if (reportBuf[0] != 0x30) return -1; // no gyro data

        // read raw IMU values
        var dt = reportBuf[1] - _timestamp;
        if (reportBuf[1] < _timestamp) dt += 0x100;

        for (var n = 0; n < 3; ++n)
        {
            ExtractImuValues(reportBuf, n);

            var dtSec = 0.005f * dt;
            _sum[0] += (float)(_gyrG.X * dtSec);
            _sum[1] += (float)(_gyrG.Y * dtSec);
            _sum[2] += (float)(_gyrG.Z * dtSec);

            if (IsLeft)
            {
                _gyrG.Y *= -1;
                _gyrG.Z *= -1;
                _accG.Y *= -1;
                _accG.Z *= -1;
            }

            if (_firstImuPacket)
            {
                Ib = new Vector3(1, 0, 0);
                Jb = new Vector3(0, 1, 0);
                Kb = new Vector3(0, 0, 1);
                _firstImuPacket = false;
            }
            else
            {
                KAcc = -Vector3.Normalize(_accG);
                _wA = Vector3.Cross(Kb, KAcc);
                _wG = -_gyrG * dtSec;
                _dTheta = (_filterweight * _wA + _wG) / (1f + _filterweight);
                Kb += Vector3.Cross(_dTheta, Kb);
                Ib += Vector3.Cross(_dTheta, Ib);
                Jb += Vector3.Cross(_dTheta, Jb);
                //Correction, ensure new axes are orthogonal
                _err = (float)(Vector3.Dot(Ib, Jb) * 0.5);
                _iB = Vector3.Normalize(Ib - _err * Jb);
                Jb = Vector3.Normalize(Jb - _err * Ib);
                Ib = _iB;
                Kb = Vector3.Cross(Ib, Jb);
            }

            dt = 1;
        }

        _timestamp = reportBuf[1] + 2;
        return 0;
    }

    public void Begin()
    {
        if (_pollThreadObj == null)
        {
            _pollThreadObj = new Thread(Poll);
            _pollThreadObj.Start();
        }
    }

    public void Recenter()
    {
        _firstImuPacket = true;
    }

    private float[] CenterSticks(ushort[] vals)
    {
        float[] s = [0, 0];
        for (uint i = 0; i < 2; ++i)
        {
            float diff = vals[i] - _stickCal[2 + i];
            if (Math.Abs(diff) < _deadzone) vals[i] = 0;
            else if (diff > 0) // if axis is above center
                s[i] = diff / _stickCal[i];
            else
                s[i] = diff / _stickCal[4 + i];
        }

        return s;
    }

    public void SetRumble(float lowFreq, float highFreq, float amp, int time = 0)
    {
        if (ControllerState <= State.ATTACHED) return;
        if (_rumbleObj.TimedRumble == false || _rumbleObj.T < 0) _rumbleObj = new Rumble(lowFreq, highFreq, amp, time);
    }

    public void TryRumble()
    {
        if (_rumbleObj.GetData() is null) return;
        SendRumble(_rumbleObj.GetData());
    }

    private void SendRumble(byte[] buffer)
    {
        var buf = new byte[ReportLen];
        buf[0] = 0x10;
        buf[1] = _globalCount;
        if (_globalCount == 0xf) _globalCount = 0;
        else ++_globalCount;
        Array.Copy(buffer, 0, buf, 2, 8);
        PrintArray(buf, DebugType.RUMBLE, format: "Rumble data sent: {0:S}");
        HidApi.hid_write(_handle, buf, new UIntPtr(ReportLen));
    }

    private byte[] Subcommand(byte sc, byte[] buffer, uint len, bool print = true)
    {
        var buf = new byte[ReportLen];
        var response = new byte[ReportLen];
        Array.Copy(_defaultBuf, 0, buf, 2, 8);
        Array.Copy(buffer, 0, buf, 11, len);
        buf[10] = sc;
        buf[1] = _globalCount;
        buf[0] = 0x1;
        if (_globalCount == 0xf) _globalCount = 0;
        else ++_globalCount;
        if (print) PrintArray(buf, DebugType.COMMS, len, 11, "Subcommand 0x" + $"{sc:X2}" + " sent. Data: 0x{0:S}");
        ;
        HidApi.hid_write(_handle, buf, new UIntPtr(len + 11));
        var res = HidApi.hid_read_timeout(_handle, response, new UIntPtr(ReportLen), 50);
        if (res < 1) DebugPrint("No response.", DebugType.COMMS);
        else if (print)
            PrintArray(response, DebugType.COMMS, ReportLen - 1, 1, "Response ID 0x" + $"{response[0]:X2}" + ". Data: 0x{0:S}");
        return response;
    }

    private void dump_calibration_data()
    {
        var buf = ReadSpi(0x80, IsLeft ? (byte)0x12 : (byte)0x1d, 9); // get user calibration data if possible
        var found = false;
        for (var i = 0; i < 9; ++i)
            if (buf[i] != 0xff)
            {
                Debug.WriteLine("Using user stick calibration data.");
                found = true;
                break;
            }

        if (!found)
        {
            Debug.WriteLine("Using factory stick calibration data.");
            buf = ReadSpi(0x60, IsLeft ? (byte)0x3d : (byte)0x46, 9); // get user calibration data if possible
        }

        _stickCal[IsLeft ? 0 : 2] = (ushort)(((buf[1] << 8) & 0xF00) | buf[0]); // X Axis Max above center
        _stickCal[IsLeft ? 1 : 3] = (ushort)((buf[2] << 4) | (buf[1] >> 4)); // Y Axis Max above center
        _stickCal[IsLeft ? 2 : 4] = (ushort)(((buf[4] << 8) & 0xF00) | buf[3]); // X Axis Center
        _stickCal[IsLeft ? 3 : 5] = (ushort)((buf[5] << 4) | (buf[4] >> 4)); // Y Axis Center
        _stickCal[IsLeft ? 4 : 0] = (ushort)(((buf[7] << 8) & 0xF00) | buf[6]); // X Axis Min below center
        _stickCal[IsLeft ? 5 : 1] = (ushort)((buf[8] << 4) | (buf[7] >> 4)); // Y Axis Min below center

        PrintArray(_stickCal, len: 6, start: 0, format: "Stick calibration data: {0:S}");

        buf = ReadSpi(0x60, IsLeft ? (byte)0x86 : (byte)0x98, 16);
        _deadzone = (ushort)(((buf[4] << 8) & 0xF00) | buf[3]);

        buf = ReadSpi(0x80, 0x34, 10);
        _gyrNeutral[0] = (short)(buf[0] | ((buf[1] << 8) & 0xff00));
        _gyrNeutral[1] = (short)(buf[2] | ((buf[3] << 8) & 0xff00));
        _gyrNeutral[2] = (short)(buf[4] | ((buf[5] << 8) & 0xff00));
        PrintArray(_gyrNeutral, len: 3, d: DebugType.IMU, format: "User gyro neutral position: {0:S}");

        // This is an extremely messy way of checking to see whether there is user stick calibration data present, but I've seen conflicting user calibration data on blank Joy-Cons. Worth another look eventually.
        if (_gyrNeutral[0] + _gyrNeutral[1] + _gyrNeutral[2] == -3 || Math.Abs(_gyrNeutral[0]) > 100 || Math.Abs(_gyrNeutral[1]) > 100 ||
            Math.Abs(_gyrNeutral[2]) > 100)
        {
            buf = ReadSpi(0x60, 0x29, 10);
            _gyrNeutral[0] = (short)(buf[3] | ((buf[4] << 8) & 0xff00));
            _gyrNeutral[1] = (short)(buf[5] | ((buf[6] << 8) & 0xff00));
            _gyrNeutral[2] = (short)(buf[7] | ((buf[8] << 8) & 0xff00));
            PrintArray(_gyrNeutral, len: 3, d: DebugType.IMU, format: "Factory gyro neutral position: {0:S}");
        }
    }

    private byte[] ReadSpi(byte addr1, byte addr2, uint len, bool print = false)
    {
        byte[] buffer = [addr2, addr1, 0x00, 0x00, (byte)len];
        var readBuf = new byte[len];
        var buf = new byte[len + 20];

        for (var i = 0; i < 100; ++i)
        {
            buf = Subcommand(0x10, buffer, 5, false);
            if (buf[15] == addr2 && buf[16] == addr1) break;
        }

        Array.Copy(buf, 20, readBuf, 0, len);
        if (print) PrintArray(readBuf, DebugType.COMMS, len);
        return readBuf;
    }

    private void PrintArray<T>(T[] arr, DebugType d = DebugType.NONE, uint len = 0, uint start = 0, string format = "{0:S}")
    {
        if (d != ControllerDebugType && ControllerDebugType != DebugType.ALL) return;
        if (len == 0) len = (uint)arr.Length;
        var tostr = "";
        for (var i = 0; i < len; ++i) tostr += string.Format(arr[0] is byte ? "{0:X2} " : arr[0] is float ? "{0:F} " : "{0:D} ", arr[i + start]);
        DebugPrint(string.Format(format, tostr), d);
    }

    private struct Report
    {
        private readonly byte[] _r;
        private readonly DateTime _t;

        public Report(byte[] report, DateTime time)
        {
            _r = report;
            _t = time;
        }

        public DateTime GetTime()
        {
            return _t;
        }

        public void CopyBuffer(byte[] b)
        {
            for (var i = 0; i < ReportLen; ++i) b[i] = _r[i];
        }
    }

    private struct Rumble
    {
        private float _hF, _amp, _lF;
        public float T;
        public bool TimedRumble;

        public void set_vals(float lowFreq, float highFreq, float amplitude, int time = 0)
        {
            _hF = highFreq;
            _amp = amplitude;
            _lF = lowFreq;
            TimedRumble = false;
            T = 0;
            if (time != 0)
            {
                T = time / 1000f;
                TimedRumble = true;
            }
        }

        public Rumble(float lowFreq, float highFreq, float amplitude, int time = 0)
        {
            _hF = highFreq;
            _amp = amplitude;
            _lF = lowFreq;
            TimedRumble = false;
            T = 0;
            if (time != 0)
            {
                T = time / 1000f;
                TimedRumble = true;
            }
        }

        private float Clamp(float x, float min, float max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }

        public byte[] GetData()
        {
            var rumbleData = new byte[8];
            _lF = Clamp(_lF, 40.875885f, 626.286133f);
            _amp = Clamp(_amp, 0.0f, 1.0f);
            _hF = Clamp(_hF, 81.75177f, 1252.572266f);
            var hf = (ushort)((Math.Round(32f * Math.Log(_hF * 0.1f, 2)) - 0x60) * 4);
            var lf = (byte)(Math.Round(32f * Math.Log(_lF * 0.1f, 2)) - 0x40);
            byte hfAmp;
            if (_amp == 0) hfAmp = 0;
            else if (_amp < 0.117) hfAmp = (byte)((Math.Log(_amp * 1000, 2) * 32 - 0x60) / (5 - Math.Pow(_amp, 2)) - 1);
            else if (_amp < 0.23) hfAmp = (byte)(Math.Log(_amp * 1000, 2) * 32 - 0x60 - 0x5c);
            else hfAmp = (byte)((Math.Log(_amp * 1000, 2) * 32 - 0x60) * 2 - 0xf6);

            var lfAmp = (ushort)(hfAmp * .5);
            var parity = (byte)(lfAmp % 2);
            if (parity > 0) --lfAmp;

            lfAmp = (ushort)(lfAmp >> 1);
            lfAmp += 0x40;
            if (parity > 0) lfAmp |= 0x8000;
            rumbleData = new byte[8];
            rumbleData[0] = (byte)(hf & 0xff);
            rumbleData[1] = (byte)((hf >> 8) & 0xff);
            rumbleData[2] = lf;
            rumbleData[1] += hfAmp;
            rumbleData[2] += (byte)((lfAmp >> 8) & 0xff);
            rumbleData[3] += (byte)(lfAmp & 0xff);
            for (var i = 0; i < 4; ++i) rumbleData[4 + i] = rumbleData[i];
            //Debug.WriteLine(string.Format("Encoded hex freq: {0:X2}", encoded_hex_freq));
            //Debug.Log(string.Format("lf_amp: {0:X4}", lf_amp));
            //Debug.Log(string.Format("hf_amp: {0:X2}", hf_amp));
            //Debug.Log(string.Format("l_f: {0:F}", l_f));
            //Debug.Log(string.Format("hf: {0:X4}", hf));
            //Debug.Log(string.Format("lf: {0:X2}", lf));
            return rumbleData;
        }
    }
}