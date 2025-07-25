﻿/*
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

public struct Matrix : IEquatable<Matrix>
{
    #region Public Constructors

    public Matrix(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31,
        double m32, double m33, double m34, double m41, double m42, double m43, double m44)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;
        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;
        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;
    }

    #endregion Public Constructors

    #region Public Fields

    public double M11;
    public double M12;
    public double M13;
    public double M14;
    public double M21;
    public double M22;
    public double M23;
    public double M24;
    public double M31;
    public double M32;
    public double M33;
    public double M34;
    public double M41;
    public double M42;
    public double M43;
    public double M44;

    #endregion Public Fields

    #region Public Properties

    public Vector3 Backward
    {
        get => new(M31, M32, M33);
        set
        {
            M31 = value.X;
            M32 = value.Y;
            M33 = value.Z;
        }
    }


    public Vector3 Down
    {
        get => new(-M21, -M22, -M23);
        set
        {
            M21 = -value.X;
            M22 = -value.Y;
            M23 = -value.Z;
        }
    }


    public Vector3 Forward
    {
        get => new(-M31, -M32, -M33);
        set
        {
            M31 = -value.X;
            M32 = -value.Y;
            M33 = -value.Z;
        }
    }


    public static Matrix Identity { get; } = new(1f, 0f, 0f, 0f,
        0f, 1f, 0f, 0f,
        0f, 0f, 1f, 0f,
        0f, 0f, 0f, 1f);


    // required for OpenGL 2.0 projection matrix stuff
    public static double[] ToFloatArray(Matrix mat)
    {
        double[] matarray =
        [
            mat.M11, mat.M12, mat.M13, mat.M14,
            mat.M21, mat.M22, mat.M23, mat.M24,
            mat.M31, mat.M32, mat.M33, mat.M34,
            mat.M41, mat.M42, mat.M43, mat.M44
        ];
        return matarray;
    }

    public Vector3 Left
    {
        get => new(-M11, -M12, -M13);
        set
        {
            M11 = -value.X;
            M12 = -value.Y;
            M13 = -value.Z;
        }
    }


    public Vector3 Right
    {
        get => new(M11, M12, M13);
        set
        {
            M11 = value.X;
            M12 = value.Y;
            M13 = value.Z;
        }
    }


    public Vector3 Translation
    {
        get => new(M41, M42, M43);
        set
        {
            M41 = value.X;
            M42 = value.Y;
            M43 = value.Z;
        }
    }


    public Vector3 Up
    {
        get => new(M21, M22, M23);
        set
        {
            M21 = value.X;
            M22 = value.Y;
            M23 = value.Z;
        }
    }

    #endregion Public Properties

    #region Public Methods

    public static Matrix Add(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 += matrix2.M11;
        matrix1.M12 += matrix2.M12;
        matrix1.M13 += matrix2.M13;
        matrix1.M14 += matrix2.M14;
        matrix1.M21 += matrix2.M21;
        matrix1.M22 += matrix2.M22;
        matrix1.M23 += matrix2.M23;
        matrix1.M24 += matrix2.M24;
        matrix1.M31 += matrix2.M31;
        matrix1.M32 += matrix2.M32;
        matrix1.M33 += matrix2.M33;
        matrix1.M34 += matrix2.M34;
        matrix1.M41 += matrix2.M41;
        matrix1.M42 += matrix2.M42;
        matrix1.M43 += matrix2.M43;
        matrix1.M44 += matrix2.M44;
        return matrix1;
    }


    public static void Add(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        result.M11 = matrix1.M11 + matrix2.M11;
        result.M12 = matrix1.M12 + matrix2.M12;
        result.M13 = matrix1.M13 + matrix2.M13;
        result.M14 = matrix1.M14 + matrix2.M14;
        result.M21 = matrix1.M21 + matrix2.M21;
        result.M22 = matrix1.M22 + matrix2.M22;
        result.M23 = matrix1.M23 + matrix2.M23;
        result.M24 = matrix1.M24 + matrix2.M24;
        result.M31 = matrix1.M31 + matrix2.M31;
        result.M32 = matrix1.M32 + matrix2.M32;
        result.M33 = matrix1.M33 + matrix2.M33;
        result.M34 = matrix1.M34 + matrix2.M34;
        result.M41 = matrix1.M41 + matrix2.M41;
        result.M42 = matrix1.M42 + matrix2.M42;
        result.M43 = matrix1.M43 + matrix2.M43;
        result.M44 = matrix1.M44 + matrix2.M44;
    }


    public static Matrix CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition,
        Vector3 cameraUpVector, Vector3? cameraForwardVector)
    {
        var diff = cameraPosition - objectPosition;

        var matrix = Identity;

        diff.Normalize();
        matrix.Forward = diff;
        matrix.Left = Vector3.Cross(diff, cameraUpVector);
        matrix.Up = cameraUpVector;
        matrix.Translation = objectPosition;

        return matrix;

        /*Matrix matrix;
        Vector3 vector;
        Vector3 vector2;
        Vector3 vector3;
        vector.X = objectPosition.X - cameraPosition.X;
        vector.Y = objectPosition.Y - cameraPosition.Y;
        vector.Z = objectPosition.Z - cameraPosition.Z;
        double num = vector.LengthSquared();
        if (num < 0.0001f)
        {
            vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        }
        else
        {
            Vector3.Multiply(ref vector, (double) (1f / ((double) Math.Sqrt((double) num))), out vector);
        }
        Vector3.Cross(ref cameraUpVector, ref vector, out vector3);
        vector3.Normalize();
        Vector3.Cross(ref vector, ref vector3, out vector2);
        matrix.M11 = vector3.X;
        matrix.M12 = vector3.Y;
        matrix.M13 = vector3.Z;
        matrix.M14 = 0f;
        matrix.M21 = vector2.X;
        matrix.M22 = vector2.Y;
        matrix.M23 = vector2.Z;
        matrix.M24 = 0f;
        matrix.M31 = vector.X;
        matrix.M32 = vector.Y;
        matrix.M33 = vector.Z;
        matrix.M34 = 0f;
        matrix.M41 = objectPosition.X;
        matrix.M42 = objectPosition.Y;
        matrix.M43 = objectPosition.Z;
        matrix.M44 = 1f;
        return matrix;*/
    }


    public static void CreateBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
        ref Vector3 cameraUpVector, Vector3? cameraForwardVector, out Matrix result)
    {
        Vector3 vector;
        Vector3 vector2;
        Vector3 vector3;
        vector.X = objectPosition.X - cameraPosition.X;
        vector.Y = objectPosition.Y - cameraPosition.Y;
        vector.Z = objectPosition.Z - cameraPosition.Z;
        var num = vector.LengthSquared();
        if (num < 0.0001f)
            vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        else
            Vector3.Multiply(ref vector, 1f / Math.Sqrt(num), out vector);
        Vector3.Cross(ref cameraUpVector, ref vector, out vector3);
        vector3.Normalize();
        Vector3.Cross(ref vector, ref vector3, out vector2);
        result.M11 = vector3.X;
        result.M12 = vector3.Y;
        result.M13 = vector3.Z;
        result.M14 = 0f;
        result.M21 = vector2.X;
        result.M22 = vector2.Y;
        result.M23 = vector2.Z;
        result.M24 = 0f;
        result.M31 = vector.X;
        result.M32 = vector.Y;
        result.M33 = vector.Z;
        result.M34 = 0f;
        result.M41 = objectPosition.X;
        result.M42 = objectPosition.Y;
        result.M43 = objectPosition.Z;
        result.M44 = 1f;
    }


    public static Matrix CreateConstrainedBillboard(Vector3 objectPosition, Vector3 cameraPosition,
        Vector3 rotateAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector)
    {
        double num;
        Vector3 vector;
        Matrix matrix;
        Vector3 vector2;
        Vector3 vector3;
        vector2.X = objectPosition.X - cameraPosition.X;
        vector2.Y = objectPosition.Y - cameraPosition.Y;
        vector2.Z = objectPosition.Z - cameraPosition.Z;
        var num2 = vector2.LengthSquared();
        if (num2 < 0.0001f)
            vector2 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        else
            Vector3.Multiply(ref vector2, 1f / Math.Sqrt(num2), out vector2);
        var vector4 = rotateAxis;
        Vector3.Dot(ref rotateAxis, ref vector2, out num);
        if (Math.Abs(num) > 0.9982547f)
        {
            if (objectForwardVector.HasValue)
            {
                vector = objectForwardVector.Value;
                Vector3.Dot(ref rotateAxis, ref vector, out num);
                if (Math.Abs(num) > 0.9982547f)
                {
                    num = rotateAxis.X * Vector3.Forward.X + rotateAxis.Y * Vector3.Forward.Y + rotateAxis.Z * Vector3.Forward.Z;
                    vector = Math.Abs(num) > 0.9982547f ? Vector3.Right : Vector3.Forward;
                }
            }
            else
            {
                num = rotateAxis.X * Vector3.Forward.X + rotateAxis.Y * Vector3.Forward.Y + rotateAxis.Z * Vector3.Forward.Z;
                vector = Math.Abs(num) > 0.9982547f ? Vector3.Right : Vector3.Forward;
            }

            Vector3.Cross(ref rotateAxis, ref vector, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector3, ref rotateAxis, out vector);
            vector.Normalize();
        }
        else
        {
            Vector3.Cross(ref rotateAxis, ref vector2, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector3, ref vector4, out vector);
            vector.Normalize();
        }

        matrix.M11 = vector3.X;
        matrix.M12 = vector3.Y;
        matrix.M13 = vector3.Z;
        matrix.M14 = 0f;
        matrix.M21 = vector4.X;
        matrix.M22 = vector4.Y;
        matrix.M23 = vector4.Z;
        matrix.M24 = 0f;
        matrix.M31 = vector.X;
        matrix.M32 = vector.Y;
        matrix.M33 = vector.Z;
        matrix.M34 = 0f;
        matrix.M41 = objectPosition.X;
        matrix.M42 = objectPosition.Y;
        matrix.M43 = objectPosition.Z;
        matrix.M44 = 1f;
        return matrix;
    }


    public static void CreateConstrainedBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
        ref Vector3 rotateAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector, out Matrix result)
    {
        double num;
        Vector3 vector;
        Vector3 vector2;
        Vector3 vector3;
        vector2.X = objectPosition.X - cameraPosition.X;
        vector2.Y = objectPosition.Y - cameraPosition.Y;
        vector2.Z = objectPosition.Z - cameraPosition.Z;
        var num2 = vector2.LengthSquared();
        if (num2 < 0.0001f)
            vector2 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
        else
            Vector3.Multiply(ref vector2, 1f / Math.Sqrt(num2), out vector2);
        var vector4 = rotateAxis;
        Vector3.Dot(ref rotateAxis, ref vector2, out num);
        if (Math.Abs(num) > 0.9982547f)
        {
            if (objectForwardVector.HasValue)
            {
                vector = objectForwardVector.Value;
                Vector3.Dot(ref rotateAxis, ref vector, out num);
                if (Math.Abs(num) > 0.9982547f)
                {
                    num = rotateAxis.X * Vector3.Forward.X + rotateAxis.Y * Vector3.Forward.Y + rotateAxis.Z * Vector3.Forward.Z;
                    vector = Math.Abs(num) > 0.9982547f ? Vector3.Right : Vector3.Forward;
                }
            }
            else
            {
                num = rotateAxis.X * Vector3.Forward.X + rotateAxis.Y * Vector3.Forward.Y + rotateAxis.Z * Vector3.Forward.Z;
                vector = Math.Abs(num) > 0.9982547f ? Vector3.Right : Vector3.Forward;
            }

            Vector3.Cross(ref rotateAxis, ref vector, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector3, ref rotateAxis, out vector);
            vector.Normalize();
        }
        else
        {
            Vector3.Cross(ref rotateAxis, ref vector2, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector3, ref vector4, out vector);
            vector.Normalize();
        }

        result.M11 = vector3.X;
        result.M12 = vector3.Y;
        result.M13 = vector3.Z;
        result.M14 = 0f;
        result.M21 = vector4.X;
        result.M22 = vector4.Y;
        result.M23 = vector4.Z;
        result.M24 = 0f;
        result.M31 = vector.X;
        result.M32 = vector.Y;
        result.M33 = vector.Z;
        result.M34 = 0f;
        result.M41 = objectPosition.X;
        result.M42 = objectPosition.Y;
        result.M43 = objectPosition.Z;
        result.M44 = 1f;
    }


    public static Matrix CreateFromAxisAngle(Vector3 axis, double angle)
    {
        Matrix matrix;
        var x = axis.X;
        var y = axis.Y;
        var z = axis.Z;
        var num2 = Math.Sin(angle);
        var num = Math.Cos(angle);
        var num11 = x * x;
        var num10 = y * y;
        var num9 = z * z;
        var num8 = x * y;
        var num7 = x * z;
        var num6 = y * z;
        matrix.M11 = num11 + num * (1f - num11);
        matrix.M12 = num8 - num * num8 + num2 * z;
        matrix.M13 = num7 - num * num7 - num2 * y;
        matrix.M14 = 0f;
        matrix.M21 = num8 - num * num8 - num2 * z;
        matrix.M22 = num10 + num * (1f - num10);
        matrix.M23 = num6 - num * num6 + num2 * x;
        matrix.M24 = 0f;
        matrix.M31 = num7 - num * num7 + num2 * y;
        matrix.M32 = num6 - num * num6 - num2 * x;
        matrix.M33 = num9 + num * (1f - num9);
        matrix.M34 = 0f;
        matrix.M41 = 0f;
        matrix.M42 = 0f;
        matrix.M43 = 0f;
        matrix.M44 = 1f;
        return matrix;
    }


    public static void CreateFromAxisAngle(ref Vector3 axis, double angle, out Matrix result)
    {
        var x = axis.X;
        var y = axis.Y;
        var z = axis.Z;
        var num2 = Math.Sin(angle);
        var num = Math.Cos(angle);
        var num11 = x * x;
        var num10 = y * y;
        var num9 = z * z;
        var num8 = x * y;
        var num7 = x * z;
        var num6 = y * z;
        result.M11 = num11 + num * (1f - num11);
        result.M12 = num8 - num * num8 + num2 * z;
        result.M13 = num7 - num * num7 - num2 * y;
        result.M14 = 0f;
        result.M21 = num8 - num * num8 - num2 * z;
        result.M22 = num10 + num * (1f - num10);
        result.M23 = num6 - num * num6 + num2 * x;
        result.M24 = 0f;
        result.M31 = num7 - num * num7 + num2 * y;
        result.M32 = num6 - num * num6 - num2 * x;
        result.M33 = num9 + num * (1f - num9);
        result.M34 = 0f;
        result.M41 = 0f;
        result.M42 = 0f;
        result.M43 = 0f;
        result.M44 = 1f;
    }


    public static Matrix CreateFromQuaternion(Quaternion quaternion)
    {
        Matrix matrix;
        var num9 = quaternion.X * quaternion.X;
        var num8 = quaternion.Y * quaternion.Y;
        var num7 = quaternion.Z * quaternion.Z;
        var num6 = quaternion.X * quaternion.Y;
        var num5 = quaternion.Z * quaternion.W;
        var num4 = quaternion.Z * quaternion.X;
        var num3 = quaternion.Y * quaternion.W;
        var num2 = quaternion.Y * quaternion.Z;
        var num = quaternion.X * quaternion.W;
        matrix.M11 = 1f - 2f * (num8 + num7);
        matrix.M12 = 2f * (num6 + num5);
        matrix.M13 = 2f * (num4 - num3);
        matrix.M14 = 0f;
        matrix.M21 = 2f * (num6 - num5);
        matrix.M22 = 1f - 2f * (num7 + num9);
        matrix.M23 = 2f * (num2 + num);
        matrix.M24 = 0f;
        matrix.M31 = 2f * (num4 + num3);
        matrix.M32 = 2f * (num2 - num);
        matrix.M33 = 1f - 2f * (num8 + num9);
        matrix.M34 = 0f;
        matrix.M41 = 0f;
        matrix.M42 = 0f;
        matrix.M43 = 0f;
        matrix.M44 = 1f;
        return matrix;
    }


    public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix result)
    {
        var num9 = quaternion.X * quaternion.X;
        var num8 = quaternion.Y * quaternion.Y;
        var num7 = quaternion.Z * quaternion.Z;
        var num6 = quaternion.X * quaternion.Y;
        var num5 = quaternion.Z * quaternion.W;
        var num4 = quaternion.Z * quaternion.X;
        var num3 = quaternion.Y * quaternion.W;
        var num2 = quaternion.Y * quaternion.Z;
        var num = quaternion.X * quaternion.W;
        result.M11 = 1f - 2f * (num8 + num7);
        result.M12 = 2f * (num6 + num5);
        result.M13 = 2f * (num4 - num3);
        result.M14 = 0f;
        result.M21 = 2f * (num6 - num5);
        result.M22 = 1f - 2f * (num7 + num9);
        result.M23 = 2f * (num2 + num);
        result.M24 = 0f;
        result.M31 = 2f * (num4 + num3);
        result.M32 = 2f * (num2 - num);
        result.M33 = 1f - 2f * (num8 + num9);
        result.M34 = 0f;
        result.M41 = 0f;
        result.M42 = 0f;
        result.M43 = 0f;
        result.M44 = 1f;
    }

    public static Matrix CreateFromYawPitchRoll(double yaw, double pitch, double roll)
    {
        Matrix matrix;
        Quaternion quaternion;
        Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
        CreateFromQuaternion(ref quaternion, out matrix);
        return matrix;
    }

    public static void CreateFromYawPitchRoll(
        double yaw,
        double pitch,
        double roll,
        out Matrix result)
    {
        Quaternion quaternion;
        Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
        CreateFromQuaternion(ref quaternion, out result);
    }

    public static Matrix CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
    {
        var vector31 = Vector3.Normalize(cameraPosition - cameraTarget);
        var vector32 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector31));
        var vector1 = Vector3.Cross(vector31, vector32);
        Matrix matrix;
        matrix.M11 = vector32.X;
        matrix.M12 = vector1.X;
        matrix.M13 = vector31.X;
        matrix.M14 = 0.0f;
        matrix.M21 = vector32.Y;
        matrix.M22 = vector1.Y;
        matrix.M23 = vector31.Y;
        matrix.M24 = 0.0f;
        matrix.M31 = vector32.Z;
        matrix.M32 = vector1.Z;
        matrix.M33 = vector31.Z;
        matrix.M34 = 0.0f;
        matrix.M41 = -Vector3.Dot(vector32, cameraPosition);
        matrix.M42 = -Vector3.Dot(vector1, cameraPosition);
        matrix.M43 = -Vector3.Dot(vector31, cameraPosition);
        matrix.M44 = 1f;
        return matrix;


        /*
        Matrix m = identity;

        m.Translation = cameraPosition;

        var diff = cameraPosition - cameraTarget;
        diff.Normalize();
        m.Forward = -diff;

        Debug.WriteLine("Forward: {0}", m.Forward);

        Vector3 right;
        Vector3.Cross(ref cameraUpVector, ref diff, out right);
        m.Right = right;

        Debug.WriteLine("Right: {0}", right);

        Vector3 up;
        Vector3.Cross(ref diff, ref right, out up);
        m.Up = up;

        return Matrix.Invert(m);
        */

        //return m;
    }


    public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix result)
    {
        var vector = Vector3.Normalize(cameraPosition - cameraTarget);
        var vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
        var vector3 = Vector3.Cross(vector, vector2);
        result.M11 = vector2.X;
        result.M12 = vector3.X;
        result.M13 = vector.X;
        result.M14 = 0f;
        result.M21 = vector2.Y;
        result.M22 = vector3.Y;
        result.M23 = vector.Y;
        result.M24 = 0f;
        result.M31 = vector2.Z;
        result.M32 = vector3.Z;
        result.M33 = vector.Z;
        result.M34 = 0f;
        result.M41 = -Vector3.Dot(vector2, cameraPosition);
        result.M42 = -Vector3.Dot(vector3, cameraPosition);
        result.M43 = -Vector3.Dot(vector, cameraPosition);
        result.M44 = 1f;
    }


    public static Matrix CreateOrthographic(double width, double height, double zNearPlane, double zFarPlane)
    {
        Matrix matrix;
        matrix.M11 = 2f / width;
        matrix.M12 = matrix.M13 = matrix.M14 = 0f;
        matrix.M22 = 2f / height;
        matrix.M21 = matrix.M23 = matrix.M24 = 0f;
        matrix.M33 = 1f / (zNearPlane - zFarPlane);
        matrix.M31 = matrix.M32 = matrix.M34 = 0f;
        matrix.M41 = matrix.M42 = 0f;
        matrix.M43 = zNearPlane / (zNearPlane - zFarPlane);
        matrix.M44 = 1f;
        return matrix;
    }


    public static void CreateOrthographic(double width, double height, double zNearPlane, double zFarPlane, out Matrix result)
    {
        result.M11 = 2f / width;
        result.M12 = result.M13 = result.M14 = 0f;
        result.M22 = 2f / height;
        result.M21 = result.M23 = result.M24 = 0f;
        result.M33 = 1f / (zNearPlane - zFarPlane);
        result.M31 = result.M32 = result.M34 = 0f;
        result.M41 = result.M42 = 0f;
        result.M43 = zNearPlane / (zNearPlane - zFarPlane);
        result.M44 = 1f;
    }


    public static Matrix CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNearPlane, double zFarPlane)
    {
        Matrix matrix;
        matrix.M11 = 2.0 / (right - left);
        matrix.M12 = 0.0f;
        matrix.M13 = 0.0f;
        matrix.M14 = 0.0f;
        matrix.M21 = 0.0f;
        matrix.M22 = 2.0 / (top - bottom);
        matrix.M23 = 0.0f;
        matrix.M24 = 0.0f;
        matrix.M31 = 0.0f;
        matrix.M32 = 0.0f;
        matrix.M33 = 1.0 / (zNearPlane - zFarPlane);
        matrix.M34 = 0.0f;
        matrix.M41 = (left + right) / (left - right);
        matrix.M42 = (top + bottom) / (bottom - top);
        matrix.M43 = zNearPlane / (zNearPlane - zFarPlane);
        matrix.M44 = 1.0f;
        return matrix;
    }


    public static void CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNearPlane, double zFarPlane,
        out Matrix result)
    {
        result.M11 = 2.0 / (right - left);
        result.M12 = 0.0f;
        result.M13 = 0.0f;
        result.M14 = 0.0f;
        result.M21 = 0.0f;
        result.M22 = 2.0 / (top - bottom);
        result.M23 = 0.0f;
        result.M24 = 0.0f;
        result.M31 = 0.0f;
        result.M32 = 0.0f;
        result.M33 = 1.0 / (zNearPlane - zFarPlane);
        result.M34 = 0.0f;
        result.M41 = (left + right) / (left - right);
        result.M42 = (top + bottom) / (bottom - top);
        result.M43 = zNearPlane / (zNearPlane - zFarPlane);
        result.M44 = 1.0f;
    }

    public static Matrix CreatePerspective(double width, double height, double nearPlaneDistance, double farPlaneDistance)
    {
        Matrix matrix;
        if (nearPlaneDistance <= 0f) throw new ArgumentException("nearPlaneDistance <= 0");
        if (farPlaneDistance <= 0f) throw new ArgumentException("farPlaneDistance <= 0");
        if (nearPlaneDistance >= farPlaneDistance) throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        matrix.M11 = 2f * nearPlaneDistance / width;
        matrix.M12 = matrix.M13 = matrix.M14 = 0f;
        matrix.M22 = 2f * nearPlaneDistance / height;
        matrix.M21 = matrix.M23 = matrix.M24 = 0f;
        matrix.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        matrix.M31 = matrix.M32 = 0f;
        matrix.M34 = -1f;
        matrix.M41 = matrix.M42 = matrix.M44 = 0f;
        matrix.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        return matrix;
    }


    public static void CreatePerspective(double width, double height, double nearPlaneDistance, double farPlaneDistance, out Matrix result)
    {
        if (nearPlaneDistance <= 0f) throw new ArgumentException("nearPlaneDistance <= 0");
        if (farPlaneDistance <= 0f) throw new ArgumentException("farPlaneDistance <= 0");
        if (nearPlaneDistance >= farPlaneDistance) throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        result.M11 = 2f * nearPlaneDistance / width;
        result.M12 = result.M13 = result.M14 = 0f;
        result.M22 = 2f * nearPlaneDistance / height;
        result.M21 = result.M23 = result.M24 = 0f;
        result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M31 = result.M32 = 0f;
        result.M34 = -1f;
        result.M41 = result.M42 = result.M44 = 0f;
        result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
    }


    public static Matrix CreatePerspectiveFieldOfView(double fieldOfView, double aspectRatio, double nearPlaneDistance, double farPlaneDistance)
    {
        Matrix matrix;
        if (fieldOfView <= 0f || fieldOfView >= 3.141593f) throw new ArgumentException("fieldOfView <= 0 O >= PI");
        if (nearPlaneDistance <= 0f) throw new ArgumentException("nearPlaneDistance <= 0");
        if (farPlaneDistance <= 0f) throw new ArgumentException("farPlaneDistance <= 0");
        if (nearPlaneDistance >= farPlaneDistance) throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        var num = 1f / Math.Tan(fieldOfView * 0.5f);
        var num9 = num / aspectRatio;
        matrix.M11 = num9;
        matrix.M12 = matrix.M13 = matrix.M14 = 0f;
        matrix.M22 = num;
        matrix.M21 = matrix.M23 = matrix.M24 = 0f;
        matrix.M31 = matrix.M32 = 0f;
        matrix.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        matrix.M34 = -1f;
        matrix.M41 = matrix.M42 = matrix.M44 = 0f;
        matrix.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        return matrix;
    }


    public static void CreatePerspectiveFieldOfView(double fieldOfView, double aspectRatio, double nearPlaneDistance, double farPlaneDistance,
        out Matrix result)
    {
        if (fieldOfView <= 0f || fieldOfView >= 3.141593f) throw new ArgumentException("fieldOfView <= 0 or >= PI");
        if (nearPlaneDistance <= 0f) throw new ArgumentException("nearPlaneDistance <= 0");
        if (farPlaneDistance <= 0f) throw new ArgumentException("farPlaneDistance <= 0");
        if (nearPlaneDistance >= farPlaneDistance) throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        var num = 1f / Math.Tan(fieldOfView * 0.5f);
        var num9 = num / aspectRatio;
        result.M11 = num9;
        result.M12 = result.M13 = result.M14 = 0f;
        result.M22 = num;
        result.M21 = result.M23 = result.M24 = 0f;
        result.M31 = result.M32 = 0f;
        result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M34 = -1f;
        result.M41 = result.M42 = result.M44 = 0f;
        result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
    }


    public static Matrix CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double nearPlaneDistance, double farPlaneDistance)
    {
        Matrix matrix;
        if (nearPlaneDistance <= 0f) throw new ArgumentException("nearPlaneDistance <= 0");
        if (farPlaneDistance <= 0f) throw new ArgumentException("farPlaneDistance <= 0");
        if (nearPlaneDistance >= farPlaneDistance) throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        matrix.M11 = 2f * nearPlaneDistance / (right - left);
        matrix.M12 = matrix.M13 = matrix.M14 = 0f;
        matrix.M22 = 2f * nearPlaneDistance / (top - bottom);
        matrix.M21 = matrix.M23 = matrix.M24 = 0f;
        matrix.M31 = (left + right) / (right - left);
        matrix.M32 = (top + bottom) / (top - bottom);
        matrix.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        matrix.M34 = -1f;
        matrix.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        matrix.M41 = matrix.M42 = matrix.M44 = 0f;
        return matrix;
    }


    public static void CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double nearPlaneDistance, double farPlaneDistance,
        out Matrix result)
    {
        if (nearPlaneDistance <= 0f) throw new ArgumentException("nearPlaneDistance <= 0");
        if (farPlaneDistance <= 0f) throw new ArgumentException("farPlaneDistance <= 0");
        if (nearPlaneDistance >= farPlaneDistance) throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
        result.M11 = 2f * nearPlaneDistance / (right - left);
        result.M12 = result.M13 = result.M14 = 0f;
        result.M22 = 2f * nearPlaneDistance / (top - bottom);
        result.M21 = result.M23 = result.M24 = 0f;
        result.M31 = (left + right) / (right - left);
        result.M32 = (top + bottom) / (top - bottom);
        result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M34 = -1f;
        result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        result.M41 = result.M42 = result.M44 = 0f;
    }


    public static Matrix CreateRotationX(double radians)
    {
        var returnMatrix = Identity;

        var val1 = Math.Cos(radians);
        var val2 = Math.Sin(radians);

        returnMatrix.M22 = val1;
        returnMatrix.M23 = val2;
        returnMatrix.M32 = -val2;
        returnMatrix.M33 = val1;

        return returnMatrix;
    }


    public static void CreateRotationX(double radians, out Matrix result)
    {
        result = Identity;

        var val1 = Math.Cos(radians);
        var val2 = Math.Sin(radians);

        result.M22 = val1;
        result.M23 = val2;
        result.M32 = -val2;
        result.M33 = val1;
    }

    public static Matrix CreateRotationY(double radians)
    {
        var returnMatrix = Identity;

        var val1 = Math.Cos(radians);
        var val2 = Math.Sin(radians);

        returnMatrix.M11 = val1;
        returnMatrix.M13 = -val2;
        returnMatrix.M31 = val2;
        returnMatrix.M33 = val1;

        return returnMatrix;
    }


    public static void CreateRotationY(double radians, out Matrix result)
    {
        result = Identity;

        var val1 = Math.Cos(radians);
        var val2 = Math.Sin(radians);

        result.M11 = val1;
        result.M13 = -val2;
        result.M31 = val2;
        result.M33 = val1;
    }


    public static Matrix CreateRotationZ(double radians)
    {
        var returnMatrix = Identity;

        var val1 = Math.Cos(radians);
        var val2 = Math.Sin(radians);

        returnMatrix.M11 = val1;
        returnMatrix.M12 = val2;
        returnMatrix.M21 = -val2;
        returnMatrix.M22 = val1;

        return returnMatrix;
    }


    public static void CreateRotationZ(double radians, out Matrix result)
    {
        result = Identity;

        var val1 = Math.Cos(radians);
        var val2 = Math.Sin(radians);

        result.M11 = val1;
        result.M12 = val2;
        result.M21 = -val2;
        result.M22 = val1;
    }


    public static Matrix CreateScale(double scale)
    {
        Matrix result;
        result.M11 = scale;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = scale;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = scale;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
        return result;
    }


    public static void CreateScale(double scale, out Matrix result)
    {
        result.M11 = scale;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = scale;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = scale;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
    }


    public static Matrix CreateScale(double xScale, double yScale, double zScale)
    {
        Matrix result;
        result.M11 = xScale;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = yScale;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = zScale;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
        return result;
    }


    public static void CreateScale(double xScale, double yScale, double zScale, out Matrix result)
    {
        result.M11 = xScale;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = yScale;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = zScale;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
    }


    public static Matrix CreateScale(Vector3 scales)
    {
        Matrix result;
        result.M11 = scales.X;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = scales.Y;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = scales.Z;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
        return result;
    }


    public static void CreateScale(ref Vector3 scales, out Matrix result)
    {
        result.M11 = scales.X;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = scales.Y;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = scales.Z;
        result.M34 = 0;
        result.M41 = 0;
        result.M42 = 0;
        result.M43 = 0;
        result.M44 = 1;
    }

    public static Matrix CreateTranslation(double xPosition, double yPosition, double zPosition)
    {
        Matrix result;
        result.M11 = 1;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = 1;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = 1;
        result.M34 = 0;
        result.M41 = xPosition;
        result.M42 = yPosition;
        result.M43 = zPosition;
        result.M44 = 1;
        return result;
    }


    public static void CreateTranslation(ref Vector3 position, out Matrix result)
    {
        result.M11 = 1;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = 1;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = 1;
        result.M34 = 0;
        result.M41 = position.X;
        result.M42 = position.Y;
        result.M43 = position.Z;
        result.M44 = 1;
    }


    public static Matrix CreateTranslation(Vector3 position)
    {
        Matrix result;
        result.M11 = 1;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = 1;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = 1;
        result.M34 = 0;
        result.M41 = position.X;
        result.M42 = position.Y;
        result.M43 = position.Z;
        result.M44 = 1;
        return result;
    }


    public static void CreateTranslation(double xPosition, double yPosition, double zPosition, out Matrix result)
    {
        result.M11 = 1;
        result.M12 = 0;
        result.M13 = 0;
        result.M14 = 0;
        result.M21 = 0;
        result.M22 = 1;
        result.M23 = 0;
        result.M24 = 0;
        result.M31 = 0;
        result.M32 = 0;
        result.M33 = 1;
        result.M34 = 0;
        result.M41 = xPosition;
        result.M42 = yPosition;
        result.M43 = zPosition;
        result.M44 = 1;
    }


    public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
    {
        Matrix ret;
        CreateWorld(ref position, ref forward, ref up, out ret);
        return ret;
    }

    public static void CreateWorld(ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix result)
    {
        Vector3 x, y, z;
        Vector3.Normalize(ref forward, out z);
        Vector3.Cross(ref forward, ref up, out x);
        Vector3.Cross(ref x, ref forward, out y);
        x.Normalize();
        y.Normalize();

        result = new Matrix();
        result.Right = x;
        result.Up = y;
        result.Forward = z;
        result.Translation = position;
        result.M44 = 1f;
    }

    public double Determinant()
    {
        var num22 = M11;
        var num21 = M12;
        var num20 = M13;
        var num19 = M14;
        var num12 = M21;
        var num11 = M22;
        var num10 = M23;
        var num9 = M24;
        var num8 = M31;
        var num7 = M32;
        var num6 = M33;
        var num5 = M34;
        var num4 = M41;
        var num3 = M42;
        var num2 = M43;
        var num = M44;
        var num18 = num6 * num - num5 * num2;
        var num17 = num7 * num - num5 * num3;
        var num16 = num7 * num2 - num6 * num3;
        var num15 = num8 * num - num5 * num4;
        var num14 = num8 * num2 - num6 * num4;
        var num13 = num8 * num3 - num7 * num4;
        return num22 * (num11 * num18 - num10 * num17 + num9 * num16) - num21 * (num12 * num18 - num10 * num15 + num9 * num14) +
            num20 * (num12 * num17 - num11 * num15 + num9 * num13) - num19 * (num12 * num16 - num11 * num14 + num10 * num13);
    }


    public static Matrix Divide(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 = matrix1.M11 / matrix2.M11;
        matrix1.M12 = matrix1.M12 / matrix2.M12;
        matrix1.M13 = matrix1.M13 / matrix2.M13;
        matrix1.M14 = matrix1.M14 / matrix2.M14;
        matrix1.M21 = matrix1.M21 / matrix2.M21;
        matrix1.M22 = matrix1.M22 / matrix2.M22;
        matrix1.M23 = matrix1.M23 / matrix2.M23;
        matrix1.M24 = matrix1.M24 / matrix2.M24;
        matrix1.M31 = matrix1.M31 / matrix2.M31;
        matrix1.M32 = matrix1.M32 / matrix2.M32;
        matrix1.M33 = matrix1.M33 / matrix2.M33;
        matrix1.M34 = matrix1.M34 / matrix2.M34;
        matrix1.M41 = matrix1.M41 / matrix2.M41;
        matrix1.M42 = matrix1.M42 / matrix2.M42;
        matrix1.M43 = matrix1.M43 / matrix2.M43;
        matrix1.M44 = matrix1.M44 / matrix2.M44;
        return matrix1;
    }


    public static void Divide(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        result.M11 = matrix1.M11 / matrix2.M11;
        result.M12 = matrix1.M12 / matrix2.M12;
        result.M13 = matrix1.M13 / matrix2.M13;
        result.M14 = matrix1.M14 / matrix2.M14;
        result.M21 = matrix1.M21 / matrix2.M21;
        result.M22 = matrix1.M22 / matrix2.M22;
        result.M23 = matrix1.M23 / matrix2.M23;
        result.M24 = matrix1.M24 / matrix2.M24;
        result.M31 = matrix1.M31 / matrix2.M31;
        result.M32 = matrix1.M32 / matrix2.M32;
        result.M33 = matrix1.M33 / matrix2.M33;
        result.M34 = matrix1.M34 / matrix2.M34;
        result.M41 = matrix1.M41 / matrix2.M41;
        result.M42 = matrix1.M42 / matrix2.M42;
        result.M43 = matrix1.M43 / matrix2.M43;
        result.M44 = matrix1.M44 / matrix2.M44;
    }


    public static Matrix Divide(Matrix matrix1, double divider)
    {
        var num = 1f / divider;
        matrix1.M11 = matrix1.M11 * num;
        matrix1.M12 = matrix1.M12 * num;
        matrix1.M13 = matrix1.M13 * num;
        matrix1.M14 = matrix1.M14 * num;
        matrix1.M21 = matrix1.M21 * num;
        matrix1.M22 = matrix1.M22 * num;
        matrix1.M23 = matrix1.M23 * num;
        matrix1.M24 = matrix1.M24 * num;
        matrix1.M31 = matrix1.M31 * num;
        matrix1.M32 = matrix1.M32 * num;
        matrix1.M33 = matrix1.M33 * num;
        matrix1.M34 = matrix1.M34 * num;
        matrix1.M41 = matrix1.M41 * num;
        matrix1.M42 = matrix1.M42 * num;
        matrix1.M43 = matrix1.M43 * num;
        matrix1.M44 = matrix1.M44 * num;
        return matrix1;
    }


    public static void Divide(ref Matrix matrix1, double divider, out Matrix result)
    {
        var num = 1f / divider;
        result.M11 = matrix1.M11 * num;
        result.M12 = matrix1.M12 * num;
        result.M13 = matrix1.M13 * num;
        result.M14 = matrix1.M14 * num;
        result.M21 = matrix1.M21 * num;
        result.M22 = matrix1.M22 * num;
        result.M23 = matrix1.M23 * num;
        result.M24 = matrix1.M24 * num;
        result.M31 = matrix1.M31 * num;
        result.M32 = matrix1.M32 * num;
        result.M33 = matrix1.M33 * num;
        result.M34 = matrix1.M34 * num;
        result.M41 = matrix1.M41 * num;
        result.M42 = matrix1.M42 * num;
        result.M43 = matrix1.M43 * num;
        result.M44 = matrix1.M44 * num;
    }


    public bool Equals(Matrix other)
    {
        return M11 == other.M11 && M22 == other.M22 && M33 == other.M33 && M44 == other.M44 && M12 == other.M12 && M13 == other.M13 && M14 == other.M14 &&
               M21 == other.M21 && M23 == other.M23 && M24 == other.M24 && M31 == other.M31 && M32 == other.M32 && M34 == other.M34 && M41 == other.M41 &&
               M42 == other.M42 && M43 == other.M43;
    }


    public override bool Equals(object obj)
    {
        var flag = false;
        if (obj is Matrix) flag = Equals((Matrix)obj);
        return flag;
    }


    public override int GetHashCode()
    {
        return M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() + M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() +
               M24.GetHashCode() + M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode() + M34.GetHashCode() + M41.GetHashCode() + M42.GetHashCode() +
               M43.GetHashCode() + M44.GetHashCode();
    }


    public static Matrix Invert(Matrix matrix)
    {
        Invert(ref matrix, out matrix);
        return matrix;
    }


    public static void Invert(ref Matrix matrix, out Matrix result)
    {
        var num1 = matrix.M11;
        var num2 = matrix.M12;
        var num3 = matrix.M13;
        var num4 = matrix.M14;
        var num5 = matrix.M21;
        var num6 = matrix.M22;
        var num7 = matrix.M23;
        var num8 = matrix.M24;
        var num9 = matrix.M31;
        var num10 = matrix.M32;
        var num11 = matrix.M33;
        var num12 = matrix.M34;
        var num13 = matrix.M41;
        var num14 = matrix.M42;
        var num15 = matrix.M43;
        var num16 = matrix.M44;
        var num17 = num11 * num16 - num12 * num15;
        var num18 = num10 * num16 - num12 * num14;
        var num19 = num10 * num15 - num11 * num14;
        var num20 = num9 * num16 - num12 * num13;
        var num21 = num9 * num15 - num11 * num13;
        var num22 = num9 * num14 - num10 * num13;
        var num23 = num6 * num17 - num7 * num18 + num8 * num19;
        var num24 = -(num5 * num17 - num7 * num20 + num8 * num21);
        var num25 = num5 * num18 - num6 * num20 + num8 * num22;
        var num26 = -(num5 * num19 - num6 * num21 + num7 * num22);
        var num27 = 1.0 / (num1 * num23 + num2 * num24 + num3 * num25 + num4 * num26);

        result.M11 = num23 * num27;
        result.M21 = num24 * num27;
        result.M31 = num25 * num27;
        result.M41 = num26 * num27;
        result.M12 = -(num2 * num17 - num3 * num18 + num4 * num19) * num27;
        result.M22 = (num1 * num17 - num3 * num20 + num4 * num21) * num27;
        result.M32 = -(num1 * num18 - num2 * num20 + num4 * num22) * num27;
        result.M42 = (num1 * num19 - num2 * num21 + num3 * num22) * num27;
        var num28 = num7 * num16 - num8 * num15;
        var num29 = num6 * num16 - num8 * num14;
        var num30 = num6 * num15 - num7 * num14;
        var num31 = num5 * num16 - num8 * num13;
        var num32 = num5 * num15 - num7 * num13;
        var num33 = num5 * num14 - num6 * num13;
        result.M13 = (num2 * num28 - num3 * num29 + num4 * num30) * num27;
        result.M23 = -(num1 * num28 - num3 * num31 + num4 * num32) * num27;
        result.M33 = (num1 * num29 - num2 * num31 + num4 * num33) * num27;
        result.M43 = -(num1 * num30 - num2 * num32 + num3 * num33) * num27;
        var num34 = num7 * num12 - num8 * num11;
        var num35 = num6 * num12 - num8 * num10;
        var num36 = num6 * num11 - num7 * num10;
        var num37 = num5 * num12 - num8 * num9;
        var num38 = num5 * num11 - num7 * num9;
        var num39 = num5 * num10 - num6 * num9;
        result.M14 = -(num2 * num34 - num3 * num35 + num4 * num36) * num27;
        result.M24 = (num1 * num34 - num3 * num37 + num4 * num38) * num27;
        result.M34 = -(num1 * num35 - num2 * num37 + num4 * num39) * num27;
        result.M44 = (num1 * num36 - num2 * num38 + num3 * num39) * num27;


        /*


        ///
        // Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
        //
        // 1. Calculate the 2x2 determinants needed the 4x4 determinant based on the 2x2 determinants
        // 3. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
        // 4. Divide adjugate matrix with the determinant to find the inverse

        double det1, det2, det3, det4, det5, det6, det7, det8, det9, det10, det11, det12;
        double detMatrix;
        findDeterminants(ref matrix, out detMatrix, out det1, out det2, out det3, out det4, out det5, out det6,
                         out det7, out det8, out det9, out det10, out det11, out det12);

        double invDetMatrix = 1f / detMatrix;

        Matrix ret; // Allow for matrix and result to point to the same structure

        ret.M11 = (matrix.M22*det12 - matrix.M23*det11 + matrix.M24*det10) * invDetMatrix;
        ret.M12 = (-matrix.M12*det12 + matrix.M13*det11 - matrix.M14*det10) * invDetMatrix;
        ret.M13 = (matrix.M42*det6 - matrix.M43*det5 + matrix.M44*det4) * invDetMatrix;
        ret.M14 = (-matrix.M32*det6 + matrix.M33*det5 - matrix.M34*det4) * invDetMatrix;
        ret.M21 = (-matrix.M21*det12 + matrix.M23*det9 - matrix.M24*det8) * invDetMatrix;
        ret.M22 = (matrix.M11*det12 - matrix.M13*det9 + matrix.M14*det8) * invDetMatrix;
        ret.M23 = (-matrix.M41*det6 + matrix.M43*det3 - matrix.M44*det2) * invDetMatrix;
        ret.M24 = (matrix.M31*det6 - matrix.M33*det3 + matrix.M34*det2) * invDetMatrix;
        ret.M31 = (matrix.M21*det11 - matrix.M22*det9 + matrix.M24*det7) * invDetMatrix;
        ret.M32 = (-matrix.M11*det11 + matrix.M12*det9 - matrix.M14*det7) * invDetMatrix;
        ret.M33 = (matrix.M41*det5 - matrix.M42*det3 + matrix.M44*det1) * invDetMatrix;
        ret.M34 = (-matrix.M31*det5 + matrix.M32*det3 - matrix.M34*det1) * invDetMatrix;
        ret.M41 = (-matrix.M21*det10 + matrix.M22*det8 - matrix.M23*det7) * invDetMatrix;
        ret.M42 = (matrix.M11*det10 - matrix.M12*det8 + matrix.M13*det7) * invDetMatrix;
        ret.M43 = (-matrix.M41*det4 + matrix.M42*det2 - matrix.M43*det1) * invDetMatrix;
        ret.M44 = (matrix.M31*det4 - matrix.M32*det2 + matrix.M33*det1) * invDetMatrix;

        result = ret;
        */
    }


    public static Matrix Lerp(Matrix matrix1, Matrix matrix2, double amount)
    {
        matrix1.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;
        matrix1.M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount;
        matrix1.M13 = matrix1.M13 + (matrix2.M13 - matrix1.M13) * amount;
        matrix1.M14 = matrix1.M14 + (matrix2.M14 - matrix1.M14) * amount;
        matrix1.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;
        matrix1.M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount;
        matrix1.M23 = matrix1.M23 + (matrix2.M23 - matrix1.M23) * amount;
        matrix1.M24 = matrix1.M24 + (matrix2.M24 - matrix1.M24) * amount;
        matrix1.M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount;
        matrix1.M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount;
        matrix1.M33 = matrix1.M33 + (matrix2.M33 - matrix1.M33) * amount;
        matrix1.M34 = matrix1.M34 + (matrix2.M34 - matrix1.M34) * amount;
        matrix1.M41 = matrix1.M41 + (matrix2.M41 - matrix1.M41) * amount;
        matrix1.M42 = matrix1.M42 + (matrix2.M42 - matrix1.M42) * amount;
        matrix1.M43 = matrix1.M43 + (matrix2.M43 - matrix1.M43) * amount;
        matrix1.M44 = matrix1.M44 + (matrix2.M44 - matrix1.M44) * amount;
        return matrix1;
    }


    public static void Lerp(ref Matrix matrix1, ref Matrix matrix2, double amount, out Matrix result)
    {
        result.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;
        result.M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount;
        result.M13 = matrix1.M13 + (matrix2.M13 - matrix1.M13) * amount;
        result.M14 = matrix1.M14 + (matrix2.M14 - matrix1.M14) * amount;
        result.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;
        result.M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount;
        result.M23 = matrix1.M23 + (matrix2.M23 - matrix1.M23) * amount;
        result.M24 = matrix1.M24 + (matrix2.M24 - matrix1.M24) * amount;
        result.M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount;
        result.M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount;
        result.M33 = matrix1.M33 + (matrix2.M33 - matrix1.M33) * amount;
        result.M34 = matrix1.M34 + (matrix2.M34 - matrix1.M34) * amount;
        result.M41 = matrix1.M41 + (matrix2.M41 - matrix1.M41) * amount;
        result.M42 = matrix1.M42 + (matrix2.M42 - matrix1.M42) * amount;
        result.M43 = matrix1.M43 + (matrix2.M43 - matrix1.M43) * amount;
        result.M44 = matrix1.M44 + (matrix2.M44 - matrix1.M44) * amount;
    }

    public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
    {
        var m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
        var m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
        var m13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
        var m14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
        var m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
        var m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
        var m23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
        var m24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
        var m31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
        var m32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
        var m33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
        var m34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
        var m41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
        var m42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
        var m43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
        var m44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
        matrix1.M11 = m11;
        matrix1.M12 = m12;
        matrix1.M13 = m13;
        matrix1.M14 = m14;
        matrix1.M21 = m21;
        matrix1.M22 = m22;
        matrix1.M23 = m23;
        matrix1.M24 = m24;
        matrix1.M31 = m31;
        matrix1.M32 = m32;
        matrix1.M33 = m33;
        matrix1.M34 = m34;
        matrix1.M41 = m41;
        matrix1.M42 = m42;
        matrix1.M43 = m43;
        matrix1.M44 = m44;
        return matrix1;
    }


    public static void Multiply(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        var m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
        var m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
        var m13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
        var m14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
        var m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
        var m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
        var m23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
        var m24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
        var m31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
        var m32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
        var m33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
        var m34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
        var m41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
        var m42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
        var m43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
        var m44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
        result.M11 = m11;
        result.M12 = m12;
        result.M13 = m13;
        result.M14 = m14;
        result.M21 = m21;
        result.M22 = m22;
        result.M23 = m23;
        result.M24 = m24;
        result.M31 = m31;
        result.M32 = m32;
        result.M33 = m33;
        result.M34 = m34;
        result.M41 = m41;
        result.M42 = m42;
        result.M43 = m43;
        result.M44 = m44;
    }

    public static Matrix Multiply(Matrix matrix1, double factor)
    {
        matrix1.M11 *= factor;
        matrix1.M12 *= factor;
        matrix1.M13 *= factor;
        matrix1.M14 *= factor;
        matrix1.M21 *= factor;
        matrix1.M22 *= factor;
        matrix1.M23 *= factor;
        matrix1.M24 *= factor;
        matrix1.M31 *= factor;
        matrix1.M32 *= factor;
        matrix1.M33 *= factor;
        matrix1.M34 *= factor;
        matrix1.M41 *= factor;
        matrix1.M42 *= factor;
        matrix1.M43 *= factor;
        matrix1.M44 *= factor;
        return matrix1;
    }


    public static void Multiply(ref Matrix matrix1, double factor, out Matrix result)
    {
        result.M11 = matrix1.M11 * factor;
        result.M12 = matrix1.M12 * factor;
        result.M13 = matrix1.M13 * factor;
        result.M14 = matrix1.M14 * factor;
        result.M21 = matrix1.M21 * factor;
        result.M22 = matrix1.M22 * factor;
        result.M23 = matrix1.M23 * factor;
        result.M24 = matrix1.M24 * factor;
        result.M31 = matrix1.M31 * factor;
        result.M32 = matrix1.M32 * factor;
        result.M33 = matrix1.M33 * factor;
        result.M34 = matrix1.M34 * factor;
        result.M41 = matrix1.M41 * factor;
        result.M42 = matrix1.M42 * factor;
        result.M43 = matrix1.M43 * factor;
        result.M44 = matrix1.M44 * factor;
    }


    public static Matrix Negate(Matrix matrix)
    {
        matrix.M11 = -matrix.M11;
        matrix.M12 = -matrix.M12;
        matrix.M13 = -matrix.M13;
        matrix.M14 = -matrix.M14;
        matrix.M21 = -matrix.M21;
        matrix.M22 = -matrix.M22;
        matrix.M23 = -matrix.M23;
        matrix.M24 = -matrix.M24;
        matrix.M31 = -matrix.M31;
        matrix.M32 = -matrix.M32;
        matrix.M33 = -matrix.M33;
        matrix.M34 = -matrix.M34;
        matrix.M41 = -matrix.M41;
        matrix.M42 = -matrix.M42;
        matrix.M43 = -matrix.M43;
        matrix.M44 = -matrix.M44;
        return matrix;
    }


    public static void Negate(ref Matrix matrix, out Matrix result)
    {
        result.M11 = -matrix.M11;
        result.M12 = -matrix.M12;
        result.M13 = -matrix.M13;
        result.M14 = -matrix.M14;
        result.M21 = -matrix.M21;
        result.M22 = -matrix.M22;
        result.M23 = -matrix.M23;
        result.M24 = -matrix.M24;
        result.M31 = -matrix.M31;
        result.M32 = -matrix.M32;
        result.M33 = -matrix.M33;
        result.M34 = -matrix.M34;
        result.M41 = -matrix.M41;
        result.M42 = -matrix.M42;
        result.M43 = -matrix.M43;
        result.M44 = -matrix.M44;
    }


    public static Matrix operator +(Matrix matrix1, Matrix matrix2)
    {
        Add(ref matrix1, ref matrix2, out matrix1);
        return matrix1;
    }


    public static Matrix operator /(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 = matrix1.M11 / matrix2.M11;
        matrix1.M12 = matrix1.M12 / matrix2.M12;
        matrix1.M13 = matrix1.M13 / matrix2.M13;
        matrix1.M14 = matrix1.M14 / matrix2.M14;
        matrix1.M21 = matrix1.M21 / matrix2.M21;
        matrix1.M22 = matrix1.M22 / matrix2.M22;
        matrix1.M23 = matrix1.M23 / matrix2.M23;
        matrix1.M24 = matrix1.M24 / matrix2.M24;
        matrix1.M31 = matrix1.M31 / matrix2.M31;
        matrix1.M32 = matrix1.M32 / matrix2.M32;
        matrix1.M33 = matrix1.M33 / matrix2.M33;
        matrix1.M34 = matrix1.M34 / matrix2.M34;
        matrix1.M41 = matrix1.M41 / matrix2.M41;
        matrix1.M42 = matrix1.M42 / matrix2.M42;
        matrix1.M43 = matrix1.M43 / matrix2.M43;
        matrix1.M44 = matrix1.M44 / matrix2.M44;
        return matrix1;
    }


    public static Matrix operator /(Matrix matrix, double divider)
    {
        var num = 1f / divider;
        matrix.M11 = matrix.M11 * num;
        matrix.M12 = matrix.M12 * num;
        matrix.M13 = matrix.M13 * num;
        matrix.M14 = matrix.M14 * num;
        matrix.M21 = matrix.M21 * num;
        matrix.M22 = matrix.M22 * num;
        matrix.M23 = matrix.M23 * num;
        matrix.M24 = matrix.M24 * num;
        matrix.M31 = matrix.M31 * num;
        matrix.M32 = matrix.M32 * num;
        matrix.M33 = matrix.M33 * num;
        matrix.M34 = matrix.M34 * num;
        matrix.M41 = matrix.M41 * num;
        matrix.M42 = matrix.M42 * num;
        matrix.M43 = matrix.M43 * num;
        matrix.M44 = matrix.M44 * num;
        return matrix;
    }


    public static bool operator ==(Matrix matrix1, Matrix matrix2)
    {
        return matrix1.M11 == matrix2.M11 &&
               matrix1.M12 == matrix2.M12 &&
               matrix1.M13 == matrix2.M13 &&
               matrix1.M14 == matrix2.M14 &&
               matrix1.M21 == matrix2.M21 &&
               matrix1.M22 == matrix2.M22 &&
               matrix1.M23 == matrix2.M23 &&
               matrix1.M24 == matrix2.M24 &&
               matrix1.M31 == matrix2.M31 &&
               matrix1.M32 == matrix2.M32 &&
               matrix1.M33 == matrix2.M33 &&
               matrix1.M34 == matrix2.M34 &&
               matrix1.M41 == matrix2.M41 &&
               matrix1.M42 == matrix2.M42 &&
               matrix1.M43 == matrix2.M43 &&
               matrix1.M44 == matrix2.M44;
    }


    public static bool operator !=(Matrix matrix1, Matrix matrix2)
    {
        return matrix1.M11 != matrix2.M11 ||
               matrix1.M12 != matrix2.M12 ||
               matrix1.M13 != matrix2.M13 ||
               matrix1.M14 != matrix2.M14 ||
               matrix1.M21 != matrix2.M21 ||
               matrix1.M22 != matrix2.M22 ||
               matrix1.M23 != matrix2.M23 ||
               matrix1.M24 != matrix2.M24 ||
               matrix1.M31 != matrix2.M31 ||
               matrix1.M32 != matrix2.M32 ||
               matrix1.M33 != matrix2.M33 ||
               matrix1.M34 != matrix2.M34 ||
               matrix1.M41 != matrix2.M41 ||
               matrix1.M42 != matrix2.M42 ||
               matrix1.M43 != matrix2.M43 ||
               matrix1.M44 != matrix2.M44;
    }


    public static Matrix operator *(Matrix matrix1, Matrix matrix2)
    {
        var m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
        var m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
        var m13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
        var m14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
        var m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
        var m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
        var m23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
        var m24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
        var m31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
        var m32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
        var m33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
        var m34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
        var m41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
        var m42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
        var m43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
        var m44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
        matrix1.M11 = m11;
        matrix1.M12 = m12;
        matrix1.M13 = m13;
        matrix1.M14 = m14;
        matrix1.M21 = m21;
        matrix1.M22 = m22;
        matrix1.M23 = m23;
        matrix1.M24 = m24;
        matrix1.M31 = m31;
        matrix1.M32 = m32;
        matrix1.M33 = m33;
        matrix1.M34 = m34;
        matrix1.M41 = m41;
        matrix1.M42 = m42;
        matrix1.M43 = m43;
        matrix1.M44 = m44;
        return matrix1;
    }


    public static Matrix operator *(Matrix matrix, double scaleFactor)
    {
        matrix.M11 = matrix.M11 * scaleFactor;
        matrix.M12 = matrix.M12 * scaleFactor;
        matrix.M13 = matrix.M13 * scaleFactor;
        matrix.M14 = matrix.M14 * scaleFactor;
        matrix.M21 = matrix.M21 * scaleFactor;
        matrix.M22 = matrix.M22 * scaleFactor;
        matrix.M23 = matrix.M23 * scaleFactor;
        matrix.M24 = matrix.M24 * scaleFactor;
        matrix.M31 = matrix.M31 * scaleFactor;
        matrix.M32 = matrix.M32 * scaleFactor;
        matrix.M33 = matrix.M33 * scaleFactor;
        matrix.M34 = matrix.M34 * scaleFactor;
        matrix.M41 = matrix.M41 * scaleFactor;
        matrix.M42 = matrix.M42 * scaleFactor;
        matrix.M43 = matrix.M43 * scaleFactor;
        matrix.M44 = matrix.M44 * scaleFactor;
        return matrix;
    }


    public static Matrix operator -(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 = matrix1.M11 - matrix2.M11;
        matrix1.M12 = matrix1.M12 - matrix2.M12;
        matrix1.M13 = matrix1.M13 - matrix2.M13;
        matrix1.M14 = matrix1.M14 - matrix2.M14;
        matrix1.M21 = matrix1.M21 - matrix2.M21;
        matrix1.M22 = matrix1.M22 - matrix2.M22;
        matrix1.M23 = matrix1.M23 - matrix2.M23;
        matrix1.M24 = matrix1.M24 - matrix2.M24;
        matrix1.M31 = matrix1.M31 - matrix2.M31;
        matrix1.M32 = matrix1.M32 - matrix2.M32;
        matrix1.M33 = matrix1.M33 - matrix2.M33;
        matrix1.M34 = matrix1.M34 - matrix2.M34;
        matrix1.M41 = matrix1.M41 - matrix2.M41;
        matrix1.M42 = matrix1.M42 - matrix2.M42;
        matrix1.M43 = matrix1.M43 - matrix2.M43;
        matrix1.M44 = matrix1.M44 - matrix2.M44;
        return matrix1;
    }


    public static Matrix operator -(Matrix matrix)
    {
        matrix.M11 = -matrix.M11;
        matrix.M12 = -matrix.M12;
        matrix.M13 = -matrix.M13;
        matrix.M14 = -matrix.M14;
        matrix.M21 = -matrix.M21;
        matrix.M22 = -matrix.M22;
        matrix.M23 = -matrix.M23;
        matrix.M24 = -matrix.M24;
        matrix.M31 = -matrix.M31;
        matrix.M32 = -matrix.M32;
        matrix.M33 = -matrix.M33;
        matrix.M34 = -matrix.M34;
        matrix.M41 = -matrix.M41;
        matrix.M42 = -matrix.M42;
        matrix.M43 = -matrix.M43;
        matrix.M44 = -matrix.M44;
        return matrix;
    }


    public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
    {
        matrix1.M11 = matrix1.M11 - matrix2.M11;
        matrix1.M12 = matrix1.M12 - matrix2.M12;
        matrix1.M13 = matrix1.M13 - matrix2.M13;
        matrix1.M14 = matrix1.M14 - matrix2.M14;
        matrix1.M21 = matrix1.M21 - matrix2.M21;
        matrix1.M22 = matrix1.M22 - matrix2.M22;
        matrix1.M23 = matrix1.M23 - matrix2.M23;
        matrix1.M24 = matrix1.M24 - matrix2.M24;
        matrix1.M31 = matrix1.M31 - matrix2.M31;
        matrix1.M32 = matrix1.M32 - matrix2.M32;
        matrix1.M33 = matrix1.M33 - matrix2.M33;
        matrix1.M34 = matrix1.M34 - matrix2.M34;
        matrix1.M41 = matrix1.M41 - matrix2.M41;
        matrix1.M42 = matrix1.M42 - matrix2.M42;
        matrix1.M43 = matrix1.M43 - matrix2.M43;
        matrix1.M44 = matrix1.M44 - matrix2.M44;
        return matrix1;
    }


    public static void Subtract(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
    {
        result.M11 = matrix1.M11 - matrix2.M11;
        result.M12 = matrix1.M12 - matrix2.M12;
        result.M13 = matrix1.M13 - matrix2.M13;
        result.M14 = matrix1.M14 - matrix2.M14;
        result.M21 = matrix1.M21 - matrix2.M21;
        result.M22 = matrix1.M22 - matrix2.M22;
        result.M23 = matrix1.M23 - matrix2.M23;
        result.M24 = matrix1.M24 - matrix2.M24;
        result.M31 = matrix1.M31 - matrix2.M31;
        result.M32 = matrix1.M32 - matrix2.M32;
        result.M33 = matrix1.M33 - matrix2.M33;
        result.M34 = matrix1.M34 - matrix2.M34;
        result.M41 = matrix1.M41 - matrix2.M41;
        result.M42 = matrix1.M42 - matrix2.M42;
        result.M43 = matrix1.M43 - matrix2.M43;
        result.M44 = matrix1.M44 - matrix2.M44;
    }


    public override string ToString()
    {
        return "{" + string.Format("M11:{0} M12:{1} M13:{2} M14:{3}", M11, M12, M13, M14) + "}"
               + " {" + string.Format("M21:{0} M22:{1} M23:{2} M24:{3}", M21, M22, M23, M24) + "}"
               + " {" + string.Format("M31:{0} M32:{1} M33:{2} M34:{3}", M31, M32, M33, M34) + "}"
               + " {" + string.Format("M41:{0} M42:{1} M43:{2} M44:{3}", M41, M42, M43, M44) + "}";
    }


    public static Matrix Transpose(Matrix matrix)
    {
        Matrix ret;
        Transpose(ref matrix, out ret);
        return ret;
    }


    public static void Transpose(ref Matrix matrix, out Matrix result)
    {
        result.M11 = matrix.M11;
        result.M12 = matrix.M21;
        result.M13 = matrix.M31;
        result.M14 = matrix.M41;

        result.M21 = matrix.M12;
        result.M22 = matrix.M22;
        result.M23 = matrix.M32;
        result.M24 = matrix.M42;

        result.M31 = matrix.M13;
        result.M32 = matrix.M23;
        result.M33 = matrix.M33;
        result.M34 = matrix.M43;

        result.M41 = matrix.M14;
        result.M42 = matrix.M24;
        result.M43 = matrix.M34;
        result.M44 = matrix.M44;
    }

    #endregion Public Methods

    #region Private Static Methods

    /// <summary>
    ///     Helper method for using the Laplace expansion theorem using two rows expansions to calculate major and
    ///     minor determinants of a 4x4 matrix. This method is used for inverting a matrix.
    /// </summary>
    private static void FindDeterminants(ref Matrix matrix, out double major,
        out double minor1, out double minor2, out double minor3, out double minor4, out double minor5, out double minor6,
        out double minor7, out double minor8, out double minor9, out double minor10, out double minor11, out double minor12)
    {
        var det1 = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
        var det2 = matrix.M11 * matrix.M23 - matrix.M13 * matrix.M21;
        var det3 = matrix.M11 * matrix.M24 - matrix.M14 * matrix.M21;
        var det4 = matrix.M12 * matrix.M23 - matrix.M13 * matrix.M22;
        var det5 = matrix.M12 * matrix.M24 - matrix.M14 * matrix.M22;
        var det6 = matrix.M13 * matrix.M24 - matrix.M14 * matrix.M23;
        var det7 = matrix.M31 * matrix.M42 - matrix.M32 * matrix.M41;
        var det8 = matrix.M31 * matrix.M43 - matrix.M33 * matrix.M41;
        var det9 = matrix.M31 * matrix.M44 - matrix.M34 * matrix.M41;
        var det10 = matrix.M32 * matrix.M43 - matrix.M33 * matrix.M42;
        var det11 = matrix.M32 * matrix.M44 - matrix.M34 * matrix.M42;
        var det12 = matrix.M33 * matrix.M44 - matrix.M34 * matrix.M43;

        major = det1 * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7;
        minor1 = det1;
        minor2 = det2;
        minor3 = det3;
        minor4 = det4;
        minor5 = det5;
        minor6 = det6;
        minor7 = det7;
        minor8 = det8;
        minor9 = det9;
        minor10 = det10;
        minor11 = det11;
        minor12 = det12;
    }

    #endregion Private Static Methods

    public bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
    {
        translation.X = M41;
        translation.Y = M42;
        translation.Z = M43;

        double xs = Math.Sign(M11 * M12 * M13 * M14) < 0 ? -1f : 1f;
        double ys = Math.Sign(M21 * M22 * M23 * M24) < 0 ? -1f : 1f;
        double zs = Math.Sign(M31 * M32 * M33 * M34) < 0 ? -1f : 1f;

        scale.X = xs * Math.Sqrt(M11 * M11 + M12 * M12 + M13 * M13);
        scale.Y = ys * Math.Sqrt(M21 * M21 + M22 * M22 + M23 * M23);
        scale.Z = zs * Math.Sqrt(M31 * M31 + M32 * M32 + M33 * M33);

        if (scale.X == 0.0 || scale.Y == 0.0 || scale.Z == 0.0)
        {
            rotation = Quaternion.Identity;
            return false;
        }

        var m1 = new Matrix(M11 / scale.X, M12 / scale.X, M13 / scale.X, 0,
            M21 / scale.Y, M22 / scale.Y, M23 / scale.Y, 0,
            M31 / scale.Z, M32 / scale.Z, M33 / scale.Z, 0,
            0, 0, 0, 1);

        rotation = Quaternion.CreateFromRotationMatrix(m1);
        return true;
    }
}