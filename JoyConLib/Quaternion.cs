using System.Text;

/*
MIT License
Copyright ÂŠ 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace JoyConLib;
#if WINRT
    [DataContract]
#else
[Serializable]
#endif
public struct Quaternion : IEquatable<Quaternion>
{
#if WINRT
        [DataMember]
#endif
    public double X;
#if WINRT
        [DataMember]
#endif
    public double Y;
#if WINRT
        [DataMember]
#endif
    public double Z;
#if WINRT
        [DataMember]
#endif
    public double W;

    private const double RadToDeg = 180.0 / Math.PI;
    private const double DegToRad = Math.PI / 180.0;


    public Quaternion(double x, double y, double z, double w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }


    public Quaternion(Vector3 vectorPart, double scalarPart)
    {
        X = vectorPart.X;
        Y = vectorPart.Y;
        Z = vectorPart.Z;
        W = scalarPart;
    }

    public static Quaternion Identity { get; } = new(0, 0, 0, 1);

    public static float Pi = (float)Math.PI;

    private static Quaternion ToQ(Vector3 euler)
    {
        var yaw = euler.Y;
        var pitch = euler.X;
        var roll = euler.Z;
        //yaw *= degToRad;
        //pitch *= degToRad;
        //roll *= degToRad;
        var rollOver2 = roll * 0.5f;
        var sinRollOver2 = Math.Sin(rollOver2);
        var cosRollOver2 = Math.Cos(rollOver2);
        var pitchOver2 = pitch * 0.5f;
        var sinPitchOver2 = Math.Sin(pitchOver2);
        var cosPitchOver2 = Math.Cos(pitchOver2);
        var yawOver2 = yaw * 0.5f;
        var sinYawOver2 = Math.Sin(yawOver2);
        var cosYawOver2 = Math.Cos(yawOver2);
        var result = new Quaternion();
        result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.X = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.Y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
        result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

        return result;
    }


    private static Vector3 FromQ(Quaternion q2)
    {
        var q = new Quaternion(q2.X, q2.Y, q2.Z, q2.W);
        var pitchYawRoll = new Vector3(0f, 0f, 0f);
        pitchYawRoll.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W)); // Yaw
        pitchYawRoll.X = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y)); // Pitch
        pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z)); // Roll
        return new Vector3(
            pitchYawRoll.X //* radToDeg
            , pitchYawRoll.Y //* radToDeg
            , pitchYawRoll.Z //* radToDeg
        );
    }

    public Vector3 EulerAngles
    {
        get => FromQ(this); // * radToDeg;
        set => this = ToQ(value); // * radToDeg);
    }


    public static Quaternion Add(Quaternion quaternion1, Quaternion quaternion2)
    {
        //Syderis
        Quaternion quaternion;
        quaternion.X = quaternion1.X + quaternion2.X;
        quaternion.Y = quaternion1.Y + quaternion2.Y;
        quaternion.Z = quaternion1.Z + quaternion2.Z;
        quaternion.W = quaternion1.W + quaternion2.W;
        return quaternion;
    }


    public static void Add(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
    {
        //Syderis
        result.X = quaternion1.X + quaternion2.X;
        result.Y = quaternion1.Y + quaternion2.Y;
        result.Z = quaternion1.Z + quaternion2.Z;
        result.W = quaternion1.W + quaternion2.W;
    }

    //Funcion aĂąadida Syderis
    public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
    {
        Quaternion quaternion;
        var x = value2.X;
        var y = value2.Y;
        var z = value2.Z;
        var w = value2.W;
        var num4 = value1.X;
        var num3 = value1.Y;
        var num2 = value1.Z;
        var num = value1.W;
        var num12 = y * num2 - z * num3;
        var num11 = z * num4 - x * num2;
        var num10 = x * num3 - y * num4;
        var num9 = x * num4 + y * num3 + z * num2;
        quaternion.X = x * num + num4 * w + num12;
        quaternion.Y = y * num + num3 * w + num11;
        quaternion.Z = z * num + num2 * w + num10;
        quaternion.W = w * num - num9;
        return quaternion;
    }

    //AĂąadida por Syderis
    public static void Concatenate(ref Quaternion value1, ref Quaternion value2, out Quaternion result)
    {
        var x = value2.X;
        var y = value2.Y;
        var z = value2.Z;
        var w = value2.W;
        var num4 = value1.X;
        var num3 = value1.Y;
        var num2 = value1.Z;
        var num = value1.W;
        var num12 = y * num2 - z * num3;
        var num11 = z * num4 - x * num2;
        var num10 = x * num3 - y * num4;
        var num9 = x * num4 + y * num3 + z * num2;
        result.X = x * num + num4 * w + num12;
        result.Y = y * num + num3 * w + num11;
        result.Z = z * num + num2 * w + num10;
        result.W = w * num - num9;
    }

    //AĂąadida por Syderis
    public void Conjugate()
    {
        X = -X;
        Y = -Y;
        Z = -Z;
    }

    //AĂąadida por Syderis
    public static Quaternion Conjugate(Quaternion value)
    {
        Quaternion quaternion;
        quaternion.X = -value.X;
        quaternion.Y = -value.Y;
        quaternion.Z = -value.Z;
        quaternion.W = value.W;
        return quaternion;
    }

    //AĂąadida por Syderis
    public static void Conjugate(ref Quaternion value, out Quaternion result)
    {
        result.X = -value.X;
        result.Y = -value.Y;
        result.Z = -value.Z;
        result.W = value.W;
    }

    internal static double Smo = 0.2;

    public void AddSmo(Quaternion q)
    {
        if (!(double.IsInfinity(q.W) || double.IsInfinity(q.X) || double.IsInfinity(q.Y) || double.IsInfinity(q.Z) ||
              double.IsNaN(q.W) || double.IsNaN(q.X) || double.IsNaN(q.Y) || double.IsNaN(q.Z)
            ))
        {
            if (double.IsInfinity(W) || double.IsInfinity(X) || double.IsInfinity(Y) || double.IsInfinity(Z) ||
                double.IsNaN(W) || double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z))
            {
                W = q.W;
                X = q.X;
                Y = q.Y;
                Z = q.Z;
            }
            else
            {
                W = Smo * q.W + (1.0 - Smo) * W;
                X = Smo * q.X + (1.0 - Smo) * X;
                Y = Smo * q.Y + (1.0 - Smo) * Y;
                Z = Smo * q.Z + (1.0 - Smo) * Z;
            }
        }
    }

    public static Vector3 AxisToEuler(Vector3 vec, double angle)
    {
        var pitchYawRoll = new Vector3();

        pitchYawRoll.X = Math.Atan2(vec.Y * Math.Sin(angle) - vec.X * vec.Z * (1 - Math.Cos(angle)),
            1 - (vec.Y * vec.Y + vec.Z * vec.Z) * (1 - Math.Cos(angle)));
        pitchYawRoll.Y = Math.Asin(vec.X * vec.Y * (1 - Math.Cos(angle)) + vec.Z * Math.Sin(angle));
        pitchYawRoll.Z = Math.Atan2(vec.X * Math.Sin(angle) - vec.Y * vec.Z * (1 - Math.Cos(angle)),
            1 - (vec.X * vec.X + vec.Z * vec.Z) * (1 - Math.Cos(angle)));


        return pitchYawRoll;
    }


    public static Vector3 ToEulerAngles8(Quaternion q)
    {
        // Store the Euler angles in radians
        var pitchYawRoll = new Vector3();

        var sqw = q.W * q.W;
        var sqx = q.X * q.X;
        var sqy = q.Y * q.Y;
        var sqz = q.Z * q.Z;

        // If quaternion is normalised the unit is one, otherwise it is the correction factor
        var unit = sqx + sqy + sqz + sqw;
        var test = q.X * q.Y + q.Z * q.W;

        if (test > 0.4999f * unit) // 0.4999f OR 0.5f - EPSILON
        {
            // Singularity at north pole
            pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = Pi * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        if (test < -0.4999f * unit) // -0.4999f OR -0.5f + EPSILON
        {
            // Singularity at south pole
            pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = -Pi * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        pitchYawRoll.Y = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, sqx - sqy - sqz + sqw); // Yaw
        pitchYawRoll.X = (float)Math.Asin(2f * test / unit); // Pitch
        pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, -sqx + sqy - sqz + sqw); // Roll

        return pitchYawRoll;
    }

    public static Quaternion CreateFromYawPitchRoll8(double yaw, double pitch, double roll)
    {
        var rollOver2 = roll * 0.5f;
        var sinRollOver2 = Math.Sin(rollOver2);
        var cosRollOver2 = Math.Cos(rollOver2);
        var pitchOver2 = pitch * 0.5f;
        var sinPitchOver2 = Math.Sin(pitchOver2);
        var cosPitchOver2 = Math.Cos(pitchOver2);
        var yawOver2 = yaw * 0.5f;
        var sinYawOver2 = Math.Sin(yawOver2);
        var cosYawOver2 = Math.Cos(yawOver2);
        Quaternion result;
        result.X = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.Y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
        result.Z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.W = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
        return result;
    }

    public static Quaternion EulerToQuaternion(double attitudeRadians, double headingRadians, double bankRadians)
    {
        // Assuming the angles are in radians.
        //(x=pitch, y=yaw, z=roll)
        var c1 = Math.Cos(attitudeRadians / 2.0);
        var s1 = Math.Sin(attitudeRadians / 2.0);
        var c2 = Math.Cos(headingRadians / 2.0);
        var s2 = Math.Sin(headingRadians / 2.0);
        var c3 = Math.Cos(bankRadians / 2.0);
        var s3 = Math.Sin(bankRadians / 2.0);
        var q = new Quaternion(
            c1 * c2 * c3 + s1 * s2 * s3, // w = cos(theta/2)
            s1 * c2 * c3 - c1 * s2 * s3, // x = v.i*sin(theta/2)
            c1 * s2 * c3 + s1 * c2 * s3, // y = v.j*sin(theta/2)
            c1 * c2 * s3 - s1 * s2 * c3); // z = v.k*sin(theta/2)

        return q;
    }

    public static Quaternion CreateFromAxisAngle(Vector3 axis, double angle)
    {
        Quaternion quaternion;
        var num2 = angle * 0.5f;
        var num = Math.Sin(num2);
        var num3 = Math.Cos(num2);
        quaternion.X = axis.X * num;
        quaternion.Y = axis.Y * num;
        quaternion.Z = axis.Z * num;
        quaternion.W = num3;
        return quaternion;
    }


    public static void CreateFromAxisAngle(ref Vector3 axis, double angle, out Quaternion result)
    {
        var num2 = angle * 0.5f;
        var num = Math.Sin(num2);
        var num3 = Math.Cos(num2);
        result.X = axis.X * num;
        result.Y = axis.Y * num;
        result.Z = axis.Z * num;
        result.W = num3;
    }


    public static Quaternion CreateFromRotationMatrix(Matrix matrix)
    {
        var num8 = matrix.M11 + matrix.M22 + matrix.M33;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = Math.Sqrt(num8 + 1f);
            quaternion.W = num * 0.5f;
            num = 0.5f / num;
            quaternion.X = (matrix.M23 - matrix.M32) * num;
            quaternion.Y = (matrix.M31 - matrix.M13) * num;
            quaternion.Z = (matrix.M12 - matrix.M21) * num;
            return quaternion;
        }

        if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
        {
            var num7 = Math.Sqrt(1f + matrix.M11 - matrix.M22 - matrix.M33);
            var num4 = 0.5f / num7;
            quaternion.X = 0.5f * num7;
            quaternion.Y = (matrix.M12 + matrix.M21) * num4;
            quaternion.Z = (matrix.M13 + matrix.M31) * num4;
            quaternion.W = (matrix.M23 - matrix.M32) * num4;
            return quaternion;
        }

        if (matrix.M22 > matrix.M33)
        {
            var num6 = Math.Sqrt(1f + matrix.M22 - matrix.M11 - matrix.M33);
            var num3 = 0.5f / num6;
            quaternion.X = (matrix.M21 + matrix.M12) * num3;
            quaternion.Y = 0.5f * num6;
            quaternion.Z = (matrix.M32 + matrix.M23) * num3;
            quaternion.W = (matrix.M31 - matrix.M13) * num3;
            return quaternion;
        }

        var num5 = Math.Sqrt(1f + matrix.M33 - matrix.M11 - matrix.M22);
        var num2 = 0.5f / num5;
        quaternion.X = (matrix.M31 + matrix.M13) * num2;
        quaternion.Y = (matrix.M32 + matrix.M23) * num2;
        quaternion.Z = 0.5f * num5;
        quaternion.W = (matrix.M12 - matrix.M21) * num2;

        return quaternion;
    }


    public static void CreateFromRotationMatrix(ref Matrix matrix, out Quaternion result)
    {
        var num8 = matrix.M11 + matrix.M22 + matrix.M33;
        if (num8 > 0f)
        {
            var num = Math.Sqrt(num8 + 1f);
            result.W = num * 0.5f;
            num = 0.5f / num;
            result.X = (matrix.M23 - matrix.M32) * num;
            result.Y = (matrix.M31 - matrix.M13) * num;
            result.Z = (matrix.M12 - matrix.M21) * num;
        }
        else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
        {
            var num7 = Math.Sqrt(1f + matrix.M11 - matrix.M22 - matrix.M33);
            var num4 = 0.5f / num7;
            result.X = 0.5f * num7;
            result.Y = (matrix.M12 + matrix.M21) * num4;
            result.Z = (matrix.M13 + matrix.M31) * num4;
            result.W = (matrix.M23 - matrix.M32) * num4;
        }
        else if (matrix.M22 > matrix.M33)
        {
            var num6 = Math.Sqrt(1f + matrix.M22 - matrix.M11 - matrix.M33);
            var num3 = 0.5f / num6;
            result.X = (matrix.M21 + matrix.M12) * num3;
            result.Y = 0.5f * num6;
            result.Z = (matrix.M32 + matrix.M23) * num3;
            result.W = (matrix.M31 - matrix.M13) * num3;
        }
        else
        {
            var num5 = Math.Sqrt(1f + matrix.M33 - matrix.M11 - matrix.M22);
            var num2 = 0.5f / num5;
            result.X = (matrix.M31 + matrix.M13) * num2;
            result.Y = (matrix.M32 + matrix.M23) * num2;
            result.Z = 0.5f * num5;
            result.W = (matrix.M12 - matrix.M21) * num2;
        }
    }

    public static Vector3 ToEulerAngles2(Quaternion q)
    {
        // Store the Euler angles in radians
        var pitchYawRoll = new Vector3();

        var sqw = q.W * q.W;
        var sqx = q.X * q.X;
        var sqy = q.Y * q.Y;
        var sqz = q.Z * q.Z;

        // If quaternion is normalised the unit is one, otherwise it is the correction factor
        var unit = sqx + sqy + sqz + sqw;
        var test = q.X * q.Y + q.Z * q.W;

        if (test > 0.4999f * unit) // 0.4999f OR 0.5f - EPSILON
        {
            // Singularity at north pole
            pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = Pi * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        if (test < -0.4999f * unit) // -0.4999f OR -0.5f + EPSILON
        {
            // Singularity at south pole
            pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = -Pi * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        pitchYawRoll.Y = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, sqx - sqy - sqz + sqw); // Yaw
        pitchYawRoll.X = (float)Math.Asin(2f * test / unit); // Pitch
        pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, -sqx + sqy - sqz + sqw); // Roll

        return pitchYawRoll;
    }

    private const double FaceRotationIncrementInDegrees = 5.0;


    public static Vector3 ExtractFaceRotation(Quaternion rotQuaternion)
    {
        var x = rotQuaternion.X;
        var y = rotQuaternion.Y;
        var z = rotQuaternion.Z;
        var w = rotQuaternion.W;

        // convert face rotation quaternion to Euler angles in degrees
        double yawD, pitchD, rollD;
        pitchD = Math.Atan2(2 * (y * z + w * x), w * w - x * x - y * y + z * z) / Math.PI * 180.0;
        yawD = Math.Asin(2 * (w * y - x * z)) / Math.PI * 180.0;
        rollD = Math.Atan2(2 * (x * y + w * z), w * w + x * x - y * y - z * z) / Math.PI * 180.0;

        // clamp the values to a multiple of the specified increment to control the refresh rate
        var increment = FaceRotationIncrementInDegrees;
        double pitch = (int)(Math.Floor((pitchD + increment / 2.0 * (pitchD > 0 ? 1.0 : -1.0)) / increment) * increment);
        double yaw = (int)(Math.Floor((yawD + increment / 2.0 * (yawD > 0 ? 1.0 : -1.0)) / increment) * increment);
        double roll = (int)(Math.Floor((rollD + increment / 2.0 * (rollD > 0 ? 1.0 : -1.0)) / increment) * increment);

        var result = new Vector3(Math.PI * pitch / 180.0, Math.PI * yaw / 180.0, Math.PI * roll / 180.0);

        return result;
    }

    public static Quaternion CreateFromYawPitchRoll2(float yaw, float pitch, float roll)
    {
        var num = roll * 0.5f;
        var num2 = (float)Math.Sin(num);
        var num3 = (float)Math.Cos(num);
        var num4 = pitch * 0.5f;
        var num5 = (float)Math.Sin(num4);
        var num6 = (float)Math.Cos(num4);
        var num7 = yaw * 0.5f;
        var num8 = (float)Math.Sin(num7);
        var num9 = (float)Math.Cos(num7);
        Quaternion result;
        result.X = num9 * num5 * num3 + num8 * num6 * num2;
        result.Y = num8 * num6 * num3 - num9 * num5 * num2;
        result.Z = num9 * num6 * num2 - num8 * num5 * num3;
        result.W = num9 * num6 * num3 + num8 * num5 * num2;
        return result;
    }

    public static Quaternion CreateFromYawPitchRoll(double yaw, double pitch, double roll)
    {
        Quaternion quaternion;
        var num9 = roll * 0.5f;
        var num6 = Math.Sin(num9);
        var num5 = Math.Cos(num9);
        var num8 = pitch * 0.5f;
        var num4 = Math.Sin(num8);
        var num3 = Math.Cos(num8);
        var num7 = yaw * 0.5f;
        var num2 = Math.Sin(num7);
        var num = Math.Cos(num7);
        quaternion.X = num * num4 * num5 + num2 * num3 * num6;
        quaternion.Y = num2 * num3 * num5 - num * num4 * num6;
        quaternion.Z = num * num3 * num6 - num2 * num4 * num5;
        quaternion.W = num * num3 * num5 + num2 * num4 * num6;
        return quaternion;
    }

    public static void CreateFromYawPitchRoll(double yaw, double pitch, double roll, out Quaternion result)
    {
        var num9 = roll * 0.5f;
        var num6 = Math.Sin(num9);
        var num5 = Math.Cos(num9);
        var num8 = pitch * 0.5f;
        var num4 = Math.Sin(num8);
        var num3 = Math.Cos(num8);
        var num7 = yaw * 0.5f;
        var num2 = Math.Sin(num7);
        var num = Math.Cos(num7);
        result.X = num * num4 * num5 + num2 * num3 * num6;
        result.Y = num2 * num3 * num5 - num * num4 * num6;
        result.Z = num * num3 * num6 - num2 * num4 * num5;
        result.W = num * num3 * num5 + num2 * num4 * num6;
    }

    public static Quaternion Divide(Quaternion quaternion1, Quaternion quaternion2)
    {
        Quaternion quaternion;
        var x = quaternion1.X;
        var y = quaternion1.Y;
        var z = quaternion1.Z;
        var w = quaternion1.W;
        var num14 = quaternion2.X * quaternion2.X + quaternion2.Y * quaternion2.Y + quaternion2.Z * quaternion2.Z + quaternion2.W * quaternion2.W;
        var num5 = 1f / num14;
        var num4 = -quaternion2.X * num5;
        var num3 = -quaternion2.Y * num5;
        var num2 = -quaternion2.Z * num5;
        var num = quaternion2.W * num5;
        var num13 = y * num2 - z * num3;
        var num12 = z * num4 - x * num2;
        var num11 = x * num3 - y * num4;
        var num10 = x * num4 + y * num3 + z * num2;
        quaternion.X = x * num + num4 * w + num13;
        quaternion.Y = y * num + num3 * w + num12;
        quaternion.Z = z * num + num2 * w + num11;
        quaternion.W = w * num - num10;
        return quaternion;
    }

    public static void Divide(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
    {
        var x = quaternion1.X;
        var y = quaternion1.Y;
        var z = quaternion1.Z;
        var w = quaternion1.W;
        var num14 = quaternion2.X * quaternion2.X + quaternion2.Y * quaternion2.Y + quaternion2.Z * quaternion2.Z + quaternion2.W * quaternion2.W;
        var num5 = 1f / num14;
        var num4 = -quaternion2.X * num5;
        var num3 = -quaternion2.Y * num5;
        var num2 = -quaternion2.Z * num5;
        var num = quaternion2.W * num5;
        var num13 = y * num2 - z * num3;
        var num12 = z * num4 - x * num2;
        var num11 = x * num3 - y * num4;
        var num10 = x * num4 + y * num3 + z * num2;
        result.X = x * num + num4 * w + num13;
        result.Y = y * num + num3 * w + num12;
        result.Z = z * num + num2 * w + num11;
        result.W = w * num - num10;
    }


    public static double Dot(Quaternion quaternion1, Quaternion quaternion2)
    {
        return quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
    }


    public static void Dot(ref Quaternion quaternion1, ref Quaternion quaternion2, out double result)
    {
        result = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
    }

    public Vector3 rotate_vector(Vector3 v)
    {
        var result = new Vector3(0, 0, 0);

        result.X = W * W * v.X + 2 * Y * W * v.Z - 2 * Z * W * v.Y + X * X * v.X + 2 * Y * X * v.Y + 2 * Z * X * v.Z - Z * Z * v.X - Y * Y * v.X;
        result.Y = 2 * X * Y * v.X + Y * Y * v.Y + 2 * Z * Y * v.Z + 2 * W * Z * v.X - Z * Z * v.Y + W * W * v.Y - 2 * X * W * v.Z - X * X * v.Y;
        result.Z = 2 * X * Z * v.X + 2 * Y * Z * v.Y + Z * Z * v.Z - 2 * W * Y * v.X - Y * Y * v.Z + 2 * W * X * v.Y - X * X * v.Z + W * W * v.Z;

        return result;
    }

    public override bool Equals(object obj)
    {
        var flag = false;
        if (obj is Quaternion) flag = Equals((Quaternion)obj);
        return flag;
    }


    public bool Equals(Quaternion other)
    {
        return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
    }


    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
    }


    public static Quaternion Inverse(Quaternion quaternion)
    {
        Quaternion quaternion2;
        var num2 = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
        var num = 1f / num2;
        quaternion2.X = -quaternion.X * num;
        quaternion2.Y = -quaternion.Y * num;
        quaternion2.Z = -quaternion.Z * num;
        quaternion2.W = quaternion.W * num;
        return quaternion2;
    }

    public static void Inverse(ref Quaternion quaternion, out Quaternion result)
    {
        var num2 = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
        var num = 1f / num2;
        result.X = -quaternion.X * num;
        result.Y = -quaternion.Y * num;
        result.Z = -quaternion.Z * num;
        result.W = quaternion.W * num;
    }

    public double Length()
    {
        var num = X * X + Y * Y + Z * Z + W * W;
        return Math.Sqrt(num);
    }


    public double LengthSquared()
    {
        return X * X + Y * Y + Z * Z + W * W;
    }


    public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
    {
        var num = amount;
        var num2 = 1f - num;
        var quaternion = new Quaternion();
        var num5 = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        if (num5 >= 0f)
        {
            quaternion.X = num2 * quaternion1.X + num * quaternion2.X;
            quaternion.Y = num2 * quaternion1.Y + num * quaternion2.Y;
            quaternion.Z = num2 * quaternion1.Z + num * quaternion2.Z;
            quaternion.W = num2 * quaternion1.W + num * quaternion2.W;
        }
        else
        {
            quaternion.X = num2 * quaternion1.X - num * quaternion2.X;
            quaternion.Y = num2 * quaternion1.Y - num * quaternion2.Y;
            quaternion.Z = num2 * quaternion1.Z - num * quaternion2.Z;
            quaternion.W = num2 * quaternion1.W - num * quaternion2.W;
        }

        var num4 = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
        var num3 = 1f / Math.Sqrt(num4);
        quaternion.X *= num3;
        quaternion.Y *= num3;
        quaternion.Z *= num3;
        quaternion.W *= num3;
        return quaternion;
    }


    public static void Lerp(ref Quaternion quaternion1, ref Quaternion quaternion2, double amount, out Quaternion result)
    {
        var num = amount;
        var num2 = 1f - num;
        var num5 = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        if (num5 >= 0f)
        {
            result.X = num2 * quaternion1.X + num * quaternion2.X;
            result.Y = num2 * quaternion1.Y + num * quaternion2.Y;
            result.Z = num2 * quaternion1.Z + num * quaternion2.Z;
            result.W = num2 * quaternion1.W + num * quaternion2.W;
        }
        else
        {
            result.X = num2 * quaternion1.X - num * quaternion2.X;
            result.Y = num2 * quaternion1.Y - num * quaternion2.Y;
            result.Z = num2 * quaternion1.Z - num * quaternion2.Z;
            result.W = num2 * quaternion1.W - num * quaternion2.W;
        }

        var num4 = result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W;
        var num3 = 1f / Math.Sqrt(num4);
        result.X *= num3;
        result.Y *= num3;
        result.Z *= num3;
        result.W *= num3;
    }


    public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
    {
        double num2;
        double num3;
        Quaternion quaternion;
        var num = amount;
        var num4 = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        var flag = false;
        if (num4 < 0f)
        {
            flag = true;
            num4 = -num4;
        }

        if (num4 > 0.999999f)
        {
            num3 = 1f - num;
            num2 = flag ? -num : num;
        }
        else
        {
            var num5 = Math.Acos(num4);
            var num6 = 1.0 / Math.Sin(num5);
            num3 = Math.Sin((1f - num) * num5) * num6;
            num2 = flag ? -Math.Sin(num * num5) * num6 : Math.Sin(num * num5) * num6;
        }

        quaternion.X = num3 * quaternion1.X + num2 * quaternion2.X;
        quaternion.Y = num3 * quaternion1.Y + num2 * quaternion2.Y;
        quaternion.Z = num3 * quaternion1.Z + num2 * quaternion2.Z;
        quaternion.W = num3 * quaternion1.W + num2 * quaternion2.W;
        return quaternion;
    }


    public static void Slerp(ref Quaternion quaternion1, ref Quaternion quaternion2, double amount, out Quaternion result)
    {
        double num2;
        double num3;
        var num = amount;
        var num4 = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        var flag = false;
        if (num4 < 0f)
        {
            flag = true;
            num4 = -num4;
        }

        if (num4 > 0.999999f)
        {
            num3 = 1f - num;
            num2 = flag ? -num : num;
        }
        else
        {
            var num5 = Math.Acos(num4);
            var num6 = 1.0 / Math.Sin(num5);
            num3 = Math.Sin((1f - num) * num5) * num6;
            num2 = flag ? -Math.Sin(num * num5) * num6 : Math.Sin(num * num5) * num6;
        }

        result.X = num3 * quaternion1.X + num2 * quaternion2.X;
        result.Y = num3 * quaternion1.Y + num2 * quaternion2.Y;
        result.Z = num3 * quaternion1.Z + num2 * quaternion2.Z;
        result.W = num3 * quaternion1.W + num2 * quaternion2.W;
    }


    public static Quaternion Subtract(Quaternion quaternion1, Quaternion quaternion2)
    {
        Quaternion quaternion;
        quaternion.X = quaternion1.X - quaternion2.X;
        quaternion.Y = quaternion1.Y - quaternion2.Y;
        quaternion.Z = quaternion1.Z - quaternion2.Z;
        quaternion.W = quaternion1.W - quaternion2.W;
        return quaternion;
    }


    public static void Subtract(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
    {
        result.X = quaternion1.X - quaternion2.X;
        result.Y = quaternion1.Y - quaternion2.Y;
        result.Z = quaternion1.Z - quaternion2.Z;
        result.W = quaternion1.W - quaternion2.W;
    }


    public static Quaternion Multiply(Quaternion quaternion1, Quaternion quaternion2)
    {
        Quaternion quaternion;
        var x = quaternion1.X;
        var y = quaternion1.Y;
        var z = quaternion1.Z;
        var w = quaternion1.W;
        var num4 = quaternion2.X;
        var num3 = quaternion2.Y;
        var num2 = quaternion2.Z;
        var num = quaternion2.W;
        var num12 = y * num2 - z * num3;
        var num11 = z * num4 - x * num2;
        var num10 = x * num3 - y * num4;
        var num9 = x * num4 + y * num3 + z * num2;
        quaternion.X = x * num + num4 * w + num12;
        quaternion.Y = y * num + num3 * w + num11;
        quaternion.Z = z * num + num2 * w + num10;
        quaternion.W = w * num - num9;
        return quaternion;
    }


    public static Quaternion Multiply(Quaternion quaternion1, double scaleFactor)
    {
        Quaternion quaternion;
        quaternion.X = quaternion1.X * scaleFactor;
        quaternion.Y = quaternion1.Y * scaleFactor;
        quaternion.Z = quaternion1.Z * scaleFactor;
        quaternion.W = quaternion1.W * scaleFactor;
        return quaternion;
    }


    public static void Multiply(ref Quaternion quaternion1, double scaleFactor, out Quaternion result)
    {
        result.X = quaternion1.X * scaleFactor;
        result.Y = quaternion1.Y * scaleFactor;
        result.Z = quaternion1.Z * scaleFactor;
        result.W = quaternion1.W * scaleFactor;
    }


    public static void Multiply(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
    {
        var x = quaternion1.X;
        var y = quaternion1.Y;
        var z = quaternion1.Z;
        var w = quaternion1.W;
        var num4 = quaternion2.X;
        var num3 = quaternion2.Y;
        var num2 = quaternion2.Z;
        var num = quaternion2.W;
        var num12 = y * num2 - z * num3;
        var num11 = z * num4 - x * num2;
        var num10 = x * num3 - y * num4;
        var num9 = x * num4 + y * num3 + z * num2;
        result.X = x * num + num4 * w + num12;
        result.Y = y * num + num3 * w + num11;
        result.Z = z * num + num2 * w + num10;
        result.W = w * num - num9;
    }


    public static Quaternion Negate(Quaternion quaternion)
    {
        Quaternion quaternion2;
        quaternion2.X = -quaternion.X;
        quaternion2.Y = -quaternion.Y;
        quaternion2.Z = -quaternion.Z;
        quaternion2.W = -quaternion.W;
        return quaternion2;
    }


    public static void Negate(ref Quaternion quaternion, out Quaternion result)
    {
        result.X = -quaternion.X;
        result.Y = -quaternion.Y;
        result.Z = -quaternion.Z;
        result.W = -quaternion.W;
    }


    public void Normalize()
    {
        var num2 = X * X + Y * Y + Z * Z + W * W;
        var num = 1f / Math.Sqrt(num2);
        X *= num;
        Y *= num;
        Z *= num;
        W *= num;
    }


    public static Quaternion Normalize(Quaternion quaternion)
    {
        Quaternion quaternion2;
        var num2 = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
        var num = 1f / Math.Sqrt(num2);
        quaternion2.X = quaternion.X * num;
        quaternion2.Y = quaternion.Y * num;
        quaternion2.Z = quaternion.Z * num;
        quaternion2.W = quaternion.W * num;
        return quaternion2;
    }


    public static void Normalize(ref Quaternion quaternion, out Quaternion result)
    {
        var num2 = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
        var num = 1f / Math.Sqrt(num2);
        result.X = quaternion.X * num;
        result.Y = quaternion.Y * num;
        result.Z = quaternion.Z * num;
        result.W = quaternion.W * num;
    }


    public static Quaternion operator +(Quaternion quaternion1, Quaternion quaternion2)
    {
        Quaternion quaternion;
        quaternion.X = quaternion1.X + quaternion2.X;
        quaternion.Y = quaternion1.Y + quaternion2.Y;
        quaternion.Z = quaternion1.Z + quaternion2.Z;
        quaternion.W = quaternion1.W + quaternion2.W;
        return quaternion;
    }


    public static Quaternion operator /(Quaternion quaternion1, Quaternion quaternion2)
    {
        Quaternion quaternion;
        var x = quaternion1.X;
        var y = quaternion1.Y;
        var z = quaternion1.Z;
        var w = quaternion1.W;
        var num14 = quaternion2.X * quaternion2.X + quaternion2.Y * quaternion2.Y + quaternion2.Z * quaternion2.Z + quaternion2.W * quaternion2.W;
        var num5 = 1f / num14;
        var num4 = -quaternion2.X * num5;
        var num3 = -quaternion2.Y * num5;
        var num2 = -quaternion2.Z * num5;
        var num = quaternion2.W * num5;
        var num13 = y * num2 - z * num3;
        var num12 = z * num4 - x * num2;
        var num11 = x * num3 - y * num4;
        var num10 = x * num4 + y * num3 + z * num2;
        quaternion.X = x * num + num4 * w + num13;
        quaternion.Y = y * num + num3 * w + num12;
        quaternion.Z = z * num + num2 * w + num11;
        quaternion.W = w * num - num10;
        return quaternion;
    }


    public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2)
    {
        return quaternion1.X == quaternion2.X && quaternion1.Y == quaternion2.Y && quaternion1.Z == quaternion2.Z && quaternion1.W == quaternion2.W;
    }


    public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2)
    {
        if (quaternion1.X == quaternion2.X && quaternion1.Y == quaternion2.Y && quaternion1.Z == quaternion2.Z) return quaternion1.W != quaternion2.W;
        return true;
    }


    public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2)
    {
        Quaternion quaternion;
        var x = quaternion1.X;
        var y = quaternion1.Y;
        var z = quaternion1.Z;
        var w = quaternion1.W;
        var num4 = quaternion2.X;
        var num3 = quaternion2.Y;
        var num2 = quaternion2.Z;
        var num = quaternion2.W;
        var num12 = y * num2 - z * num3;
        var num11 = z * num4 - x * num2;
        var num10 = x * num3 - y * num4;
        var num9 = x * num4 + y * num3 + z * num2;
        quaternion.X = x * num + num4 * w + num12;
        quaternion.Y = y * num + num3 * w + num11;
        quaternion.Z = z * num + num2 * w + num10;
        quaternion.W = w * num - num9;
        return quaternion;
    }


    public static Quaternion operator *(Quaternion quaternion1, double scaleFactor)
    {
        Quaternion quaternion;
        quaternion.X = quaternion1.X * scaleFactor;
        quaternion.Y = quaternion1.Y * scaleFactor;
        quaternion.Z = quaternion1.Z * scaleFactor;
        quaternion.W = quaternion1.W * scaleFactor;
        return quaternion;
    }


    public static Quaternion operator -(Quaternion quaternion1, Quaternion quaternion2)
    {
        Quaternion quaternion;
        quaternion.X = quaternion1.X - quaternion2.X;
        quaternion.Y = quaternion1.Y - quaternion2.Y;
        quaternion.Z = quaternion1.Z - quaternion2.Z;
        quaternion.W = quaternion1.W - quaternion2.W;
        return quaternion;
    }


    public static Quaternion operator -(Quaternion quaternion)
    {
        Quaternion quaternion2;
        quaternion2.X = -quaternion.X;
        quaternion2.Y = -quaternion.Y;
        quaternion2.Z = -quaternion.Z;
        quaternion2.W = -quaternion.W;
        return quaternion2;
    }


    public override string ToString()
    {
        var sb = new StringBuilder(32);
        sb.Append("{W:");
        sb.Append(string.Format("{0:N4}", W));
        sb.Append(" X:");
        sb.Append(string.Format("{0:N4}", X));
        sb.Append(" Y:");
        sb.Append(string.Format("{0:N4}", Y));
        sb.Append(" Z:");
        sb.Append(string.Format("{0:N4}", Z));
        sb.Append("}");
        return sb.ToString();
    }

    internal Matrix ToMatrix()
    {
        var matrix = Matrix.Identity;
        ToMatrix(out matrix);
        return matrix;
    }

    internal void ToMatrix(out Matrix matrix)
    {
        ToMatrix(this, out matrix);
    }

    internal static void ToMatrix(Quaternion quaternion, out Matrix matrix)
    {
        // source -> http://content.gpwiki.org/index.php/OpenGL:Tutorials:Using_Quaternions_to_represent_rotation#Quaternion_to_Matrix
        var x2 = quaternion.X * quaternion.X;
        var y2 = quaternion.Y * quaternion.Y;
        var z2 = quaternion.Z * quaternion.Z;
        var xy = quaternion.X * quaternion.Y;
        var xz = quaternion.X * quaternion.Z;
        var yz = quaternion.Y * quaternion.Z;
        var wx = quaternion.W * quaternion.X;
        var wy = quaternion.W * quaternion.Y;
        var wz = quaternion.W * quaternion.Z;

        // This calculation would be a lot more complicated for non-unit length quaternions
        // Note: The constructor of Matrix4 expects the Matrix in column-major format like expected by
        //   OpenGL
        matrix.M11 = 1.0f - 2.0f * (y2 + z2);
        matrix.M12 = 2.0f * (xy - wz);
        matrix.M13 = 2.0f * (xz + wy);
        matrix.M14 = 0.0f;

        matrix.M21 = 2.0f * (xy + wz);
        matrix.M22 = 1.0f - 2.0f * (x2 + z2);
        matrix.M23 = 2.0f * (yz - wx);
        matrix.M24 = 0.0f;

        matrix.M31 = 2.0f * (xz - wy);
        matrix.M32 = 2.0f * (yz + wx);
        matrix.M33 = 1.0f - 2.0f * (x2 + y2);
        matrix.M34 = 0.0f;

        matrix.M41 = 2.0f * (xz - wy);
        matrix.M42 = 2.0f * (yz + wx);
        matrix.M43 = 1.0f - 2.0f * (x2 + y2);
        matrix.M44 = 0.0f;

        //return Matrix4( 1.0f - 2.0f * (y2 + z2), 2.0f * (xy - wz), 2.0f * (xz + wy), 0.0f,
        //        2.0f * (xy + wz), 1.0f - 2.0f * (x2 + z2), 2.0f * (yz - wx), 0.0f,
        //        2.0f * (xz - wy), 2.0f * (yz + wx), 1.0f - 2.0f * (x2 + y2), 0.0f,
        //        0.0f, 0.0f, 0.0f, 1.0f)
        //    }
    }


    internal static Vector3 ToEulerAngles(Quaternion q)
    {
        // Store the Euler angles in radians
        var pitchYawRoll = new Vector3();

        var sqw = q.W * q.W;
        var sqx = q.X * q.X;
        var sqy = q.Y * q.Y;
        var sqz = q.Z * q.Z;

        // If quaternion is normalised the unit is one, otherwise it is the correction factor
        var unit = sqx + sqy + sqz + sqw;
        var test = q.X * q.Y + q.Z * q.W;

        if (test > 0.4999f * unit) // 0.4999f OR 0.5f - EPSILON
        {
            // Singularity at north pole
            pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = Math.PI * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        if (test < -0.4999f * unit) // -0.4999f OR -0.5f + EPSILON
        {
            // Singularity at south pole
            pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
            pitchYawRoll.X = -Math.PI * 0.5f; // Pitch
            pitchYawRoll.Z = 0f; // Roll
            return pitchYawRoll;
        }

        pitchYawRoll.Y = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, sqx - sqy - sqz + sqw); // Yaw
        pitchYawRoll.X = (float)Math.Asin(2f * test / unit); // Pitch
        pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, -sqx + sqy - sqz + sqw); // Roll

        return pitchYawRoll;
    }
}