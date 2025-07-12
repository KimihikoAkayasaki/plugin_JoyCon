//=============================================
// Downloaded From                            |
// Visual C# Kicks - http://www.vcskicks.com/ |
//=============================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Math3D
{
    internal class Math3D
    {
        private const double PIOVER180 = Math.PI / 180.0;

        public static Vector3D RotateX(Vector3D point3D, float degrees)
        {
            //[ a  b  c ] [ x ]   [ x*a + y*b + z*c ]
            //[ d  e  f ] [ y ] = [ x*d + y*e + z*f ]
            //[ g  h  i ] [ z ]   [ x*g + y*h + z*i ]

            //[ 1    0        0   ]
            //[ 0   cos(x)  sin(x)]
            //[ 0   -sin(x) cos(x)]

            var cDegrees = degrees * PIOVER180;
            var cosDegrees = Math.Cos(cDegrees);
            var sinDegrees = Math.Sin(cDegrees);

            var y = point3D.y * cosDegrees + point3D.z * sinDegrees;
            var z = point3D.y * -sinDegrees + point3D.z * cosDegrees;

            return new Vector3D(point3D.x, y, z);
        }

        public static Vector3D RotateY(Vector3D point3D, float degrees)
        {
            //[ cos(x)   0    sin(x)]
            //[   0      1      0   ]
            //[-sin(x)   0    cos(x)]

            var cDegrees = degrees * PIOVER180;
            var cosDegrees = Math.Cos(cDegrees);
            var sinDegrees = Math.Sin(cDegrees);

            var x = point3D.x * cosDegrees + point3D.z * sinDegrees;
            var z = point3D.x * -sinDegrees + point3D.z * cosDegrees;

            return new Vector3D(x, point3D.y, z);
        }

        public static Vector3D RotateZ(Vector3D point3D, float degrees)
        {
            //[ cos(x)  sin(x) 0]
            //[ -sin(x) cos(x) 0]
            //[    0     0     1]

            var cDegrees = degrees * PIOVER180;
            var cosDegrees = Math.Cos(cDegrees);
            var sinDegrees = Math.Sin(cDegrees);

            var x = point3D.x * cosDegrees + point3D.y * sinDegrees;
            var y = point3D.x * -sinDegrees + point3D.y * cosDegrees;

            return new Vector3D(x, y, point3D.z);
        }

        public static Vector3D Translate(Vector3D points3D, Vector3D oldOrigin, Vector3D newOrigin)
        {
            var difference = new Vector3D(newOrigin.x - oldOrigin.x, newOrigin.y - oldOrigin.y, newOrigin.z - oldOrigin.z);
            points3D.x += difference.x;
            points3D.y += difference.y;
            points3D.z += difference.z;
            return points3D;
        }

        public static Vector3D[] RotateX(Vector3D[] points3D, float degrees)
        {
            for (var i = 0; i < points3D.Length; i++) points3D[i] = RotateX(points3D[i], degrees);
            return points3D;
        }

        public static Vector3D[] RotateY(Vector3D[] points3D, float degrees)
        {
            for (var i = 0; i < points3D.Length; i++) points3D[i] = RotateY(points3D[i], degrees);
            return points3D;
        }

        public static Vector3D[] RotateZ(Vector3D[] points3D, float degrees)
        {
            for (var i = 0; i < points3D.Length; i++) points3D[i] = RotateZ(points3D[i], degrees);
            return points3D;
        }

        public static Vector3D[] Translate(Vector3D[] points3D, Vector3D oldOrigin, Vector3D newOrigin)
        {
            for (var i = 0; i < points3D.Length; i++) points3D[i] = Translate(points3D[i], oldOrigin, newOrigin);
            return points3D;
        }

        public class Vector3D
        {
            public float x;
            public float y;
            public float z;

            public Vector3D(int _x, int _y, int _z)
            {
                x = _x;
                y = _y;
                z = _z;
            }

            public Vector3D(double _x, double _y, double _z)
            {
                x = (float)_x;
                y = (float)_y;
                z = (float)_z;
            }

            public Vector3D(float _x, float _y, float _z)
            {
                x = _x;
                y = _y;
                z = _z;
            }

            public Vector3D()
            {
            }

            public override string ToString()
            {
                return "(" + x + ", " + y + ", " + z + ")";
            }
        }

        internal class Camera
        {
            public Vector3D position = new Vector3D();
        }

        public class Cube
        {
            private readonly Vector3D cubeOrigin;
            public int depth;

            private Face[] faces;
            public int height;

            public int width;

            private float xRotation;
            private float yRotation;
            private float zRotation;

            public float RotateX
            {
                get => xRotation;
                set
                {
                    //rotate the difference between this rotation and last rotation
                    RotateCubeX(-xRotation);
                    xRotation = value;
                }
            }

            public float RotateY
            {
                get => yRotation;
                set
                {
                    RotateCubeY(-yRotation);
                    yRotation = value;
                }
            }

            public float RotateZ
            {
                get => zRotation;
                set
                {
                    RotateCubeZ(-zRotation);
                    zRotation = value;
                }
            }

            public bool DrawWires { get; set; } = true;

            public bool FillFront { get; set; }

            public bool FillBack { get; set; }

            public bool FillLeft { get; set; }

            public bool FillRight { get; set; }

            public bool FillTop { get; set; }

            public bool FillBottom { get; set; }

            public void InitializeCube()
            {
                //Fill in the cube

                faces = new Face[6]; //cube has 6 faces

                //Front Face --------------------------------------------
                faces[0] = new Face();
                faces[0].CubeSide = Face.Side.Front;
                faces[0].Corners3D = new Vector3D[4];
                faces[0].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[0].Corners3D[1] = new Vector3D(0, height, 0);
                faces[0].Corners3D[2] = new Vector3D(width, height, 0);
                faces[0].Corners3D[3] = new Vector3D(width, 0, 0);
                faces[0].Center = new Vector3D(width / 2, height / 2, 0);
                // -------------------------------------------------------

                //Back Face --------------------------------------------
                faces[1] = new Face();
                faces[1].CubeSide = Face.Side.Back;
                faces[1].Corners3D = new Vector3D[4];
                faces[1].Corners3D[0] = new Vector3D(0, 0, depth);
                faces[1].Corners3D[1] = new Vector3D(0, height, depth);
                faces[1].Corners3D[2] = new Vector3D(width, height, depth);
                faces[1].Corners3D[3] = new Vector3D(width, 0, depth);
                faces[1].Center = new Vector3D(width / 2, height / 2, depth);
                // -------------------------------------------------------

                //Left Face --------------------------------------------
                faces[2] = new Face();
                faces[2].CubeSide = Face.Side.Left;
                faces[2].Corners3D = new Vector3D[4];
                faces[2].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[2].Corners3D[1] = new Vector3D(0, 0, depth);
                faces[2].Corners3D[2] = new Vector3D(0, height, depth);
                faces[2].Corners3D[3] = new Vector3D(0, height, 0);
                faces[2].Center = new Vector3D(0, height / 2, depth / 2);
                // -------------------------------------------------------

                //Right Face --------------------------------------------
                faces[3] = new Face();
                faces[3].CubeSide = Face.Side.Right;
                faces[3].Corners3D = new Vector3D[4];
                faces[3].Corners3D[0] = new Vector3D(width, 0, 0);
                faces[3].Corners3D[1] = new Vector3D(width, 0, depth);
                faces[3].Corners3D[2] = new Vector3D(width, height, depth);
                faces[3].Corners3D[3] = new Vector3D(width, height, 0);
                faces[3].Center = new Vector3D(width, height / 2, depth / 2);
                // -------------------------------------------------------

                //Top Face --------------------------------------------
                faces[4] = new Face();
                faces[4].CubeSide = Face.Side.Top;
                faces[4].Corners3D = new Vector3D[4];
                faces[4].Corners3D[0] = new Vector3D(0, 0, 0);
                faces[4].Corners3D[1] = new Vector3D(0, 0, depth);
                faces[4].Corners3D[2] = new Vector3D(width, 0, depth);
                faces[4].Corners3D[3] = new Vector3D(width, 0, 0);
                faces[4].Center = new Vector3D(width / 2, 0, depth / 2);
                // -------------------------------------------------------

                //Bottom Face --------------------------------------------
                faces[5] = new Face();
                faces[5].CubeSide = Face.Side.Bottom;
                faces[5].Corners3D = new Vector3D[4];
                faces[5].Corners3D[0] = new Vector3D(0, height, 0);
                faces[5].Corners3D[1] = new Vector3D(0, height, depth);
                faces[5].Corners3D[2] = new Vector3D(width, height, depth);
                faces[5].Corners3D[3] = new Vector3D(width, height, 0);
                faces[5].Center = new Vector3D(width / 2, height, depth / 2);
                // -------------------------------------------------------
            }

            //Calculates the 2D points of each face
            private void Update2DPoints(Point drawOrigin)
            {
                //Update the 2D points of all the faces
                for (var i = 0; i < faces.Length; i++) Update2DPoints(drawOrigin, i);
            }

            private void Update2DPoints(Point drawOrigin, int faceIndex)
            {
                //Calculates the projected coordinates of the 3D points in a cube face
                var point2D = new PointF[4];
                var zoom = Screen.PrimaryScreen.Bounds.Width / 1.5f;
                var tmpOrigin = new Point(0, 0);

                //Convert 3D Points to 2D
                Vector3D vec;
                for (var i = 0; i < point2D.Length; i++)
                {
                    vec = faces[faceIndex].Corners3D[i];
                    point2D[i] = Get2D(vec, drawOrigin);
                }

                //Update face
                faces[faceIndex].Corners2D = point2D;
            }

            //Rotating methods, has to translate the cube to the rotation point (center), rotate, and translate back

            private void RotateCubeX(float deltaX)
            {
                for (var i = 0; i < faces.Length; i++)
                {
                    //Apply rotation
                    //------Rotate points
                    var point0 = new Vector3D(0, 0, 0);
                    faces[i].Corners3D = Translate(faces[i].Corners3D, cubeOrigin, point0); //Move corner to origin
                    faces[i].Corners3D = RotateX(faces[i].Corners3D, deltaX);
                    faces[i].Corners3D = Translate(faces[i].Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    faces[i].Center = Translate(faces[i].Center, cubeOrigin, point0);
                    faces[i].Center = RotateX(faces[i].Center, deltaX);
                    faces[i].Center = Translate(faces[i].Center, point0, cubeOrigin);
                }
            }

            private void RotateCubeY(float deltaY)
            {
                for (var i = 0; i < faces.Length; i++)
                {
                    //Apply rotation
                    //------Rotate points
                    var point0 = new Vector3D(0, 0, 0);
                    faces[i].Corners3D = Translate(faces[i].Corners3D, cubeOrigin, point0); //Move corner to origin
                    faces[i].Corners3D = RotateY(faces[i].Corners3D, deltaY);
                    faces[i].Corners3D = Translate(faces[i].Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    faces[i].Center = Translate(faces[i].Center, cubeOrigin, point0);
                    faces[i].Center = RotateY(faces[i].Center, deltaY);
                    faces[i].Center = Translate(faces[i].Center, point0, cubeOrigin);
                }
            }

            private void RotateCubeZ(float deltaZ)
            {
                for (var i = 0; i < faces.Length; i++)
                {
                    //Apply rotation
                    //------Rotate points
                    var point0 = new Vector3D(0, 0, 0);
                    faces[i].Corners3D = Translate(faces[i].Corners3D, cubeOrigin, point0); //Move corner to origin
                    faces[i].Corners3D = RotateZ(faces[i].Corners3D, deltaZ);
                    faces[i].Corners3D = Translate(faces[i].Corners3D, point0, cubeOrigin); //Move back

                    //-------Rotate center
                    faces[i].Center = Translate(faces[i].Center, cubeOrigin, point0);
                    faces[i].Center = RotateZ(faces[i].Center, deltaZ);
                    faces[i].Center = Translate(faces[i].Center, point0, cubeOrigin);
                }
            }

            public Bitmap DrawCube(Point drawOrigin)
            {
                //Get the corresponding 2D
                Update2DPoints(drawOrigin);

                //Get the bounds of the final bitmap
                var bounds = getDrawingBounds();
                bounds.Width += drawOrigin.X;
                bounds.Height += drawOrigin.Y;

                var finalBmp = new Bitmap(bounds.Width, bounds.Height);
                var g = Graphics.FromImage(finalBmp);

                g.SmoothingMode = SmoothingMode.AntiAlias;

                Array.Sort(faces); //sort faces from closets to farthest
                //message();
                for (var i = faces.Length - 1; i >= 0; i--) //draw faces from back to front
                {
                    switch (faces[i].CubeSide)
                    {
                        case Face.Side.Front:
                            if (FillFront)
                                g.FillPolygon(Brushes.Gray, GetFrontFace());
                            break;
                        case Face.Side.Back:
                            if (FillBack)
                                g.FillPolygon(Brushes.DarkGray, GetBackFace());
                            break;
                        case Face.Side.Left:
                            if (FillLeft)
                                g.FillPolygon(Brushes.Gray, GetLeftFace());
                            break;
                        case Face.Side.Right:
                            if (FillRight)
                                g.FillPolygon(Brushes.DarkGray, GetRightFace());
                            break;
                        case Face.Side.Top:
                            if (FillTop)
                                g.FillPolygon(Brushes.Gray, GetTopFace());
                            break;
                        case Face.Side.Bottom:
                            if (FillBottom)
                                g.FillPolygon(Brushes.DarkGray, GetBottomFace());
                            break;
                    }

                    if (DrawWires)
                    {
                        g.DrawLine(Pens.Black, faces[i].Corners2D[0], faces[i].Corners2D[1]);
                        g.DrawLine(Pens.Black, faces[i].Corners2D[1], faces[i].Corners2D[2]);
                        g.DrawLine(Pens.Black, faces[i].Corners2D[2], faces[i].Corners2D[3]);
                        g.DrawLine(Pens.Black, faces[i].Corners2D[3], faces[i].Corners2D[0]);
                    }
                }

                g.Dispose();

                return finalBmp;
            }

            //Converts 3D points to 2D points
            private PointF Get2D(Vector3D vec, Point drawOrigin)
            {
                var point2D = Get2D(vec);
                return new PointF(point2D.X + drawOrigin.X, point2D.Y + drawOrigin.Y);
            }

            private PointF Get2D(Vector3D vec)
            {
                var returnPoint = new PointF();

                var zoom = Screen.PrimaryScreen.Bounds.Width / 1.5f;
                var tempCam = new Camera();

                tempCam.position.x = cubeOrigin.x;
                tempCam.position.y = cubeOrigin.y;
                tempCam.position.z = cubeOrigin.x * zoom / cubeOrigin.x;

                var zValue = -vec.z - tempCam.position.z;

                returnPoint.X = (tempCam.position.x - vec.x) / zValue * zoom;
                returnPoint.Y = (tempCam.position.y - vec.y) / zValue * zoom;

                return returnPoint;
            }

            public PointF[] GetFrontFace()
            {
                //Returns the four points corresponding to the front face
                //Get the corresponding 2D
                return getFace(Face.Side.Front).Corners2D;
            }

            public PointF[] GetBackFace()
            {
                return getFace(Face.Side.Back).Corners2D;
            }

            public PointF[] GetRightFace()
            {
                return getFace(Face.Side.Right).Corners2D;
            }

            public PointF[] GetLeftFace()
            {
                return getFace(Face.Side.Left).Corners2D;
            }

            public PointF[] GetTopFace()
            {
                return getFace(Face.Side.Top).Corners2D;
            }

            public PointF[] GetBottomFace()
            {
                return getFace(Face.Side.Bottom).Corners2D;
            }

            private Face getFace(Face.Side side)
            {
                //Find the correct side
                //Since faces are sorted in order of closest to farthest
                //They won't always be in the same index
                for (var i = 0; i < faces.Length; i++)
                    if (faces[i].CubeSide == side)
                        return faces[i];

                return null; //not found
            }

            private Rectangle getDrawingBounds()
            {
                //Find the farthest most points to calculate the size of the returning bitmap
                var left = float.MaxValue;
                var right = float.MinValue;
                var top = float.MaxValue;
                var bottom = float.MinValue;

                for (var i = 0; i < faces.Length; i++)
                for (var j = 0; j < faces[i].Corners2D.Length; j++)
                {
                    if (faces[i].Corners2D[j].X < left)
                        left = faces[i].Corners2D[j].X;
                    if (faces[i].Corners2D[j].X > right)
                        right = faces[i].Corners2D[j].X;
                    if (faces[i].Corners2D[j].Y < top)
                        top = faces[i].Corners2D[j].Y;
                    if (faces[i].Corners2D[j].Y > bottom)
                        bottom = faces[i].Corners2D[j].Y;
                }

                return new Rectangle(0, 0, (int)Math.Round(right - left), (int)Math.Round(bottom - top));
            }

            //Cube face, has four points, 3D and 2D
            internal class Face : IComparable<Face>
            {
                public enum Side
                {
                    Front,
                    Back,
                    Left,
                    Right,
                    Top,
                    Bottom
                }

                public Vector3D Center;

                public PointF[] Corners2D;
                public Vector3D[] Corners3D;
                public Side CubeSide;

                public int CompareTo(Face otherFace)
                {
                    return (int)(Center.z - otherFace.Center.z); //In order of which is closest to the screen
                }
            }


            #region Initializers

            public Cube(int side)
            {
                width = side;
                height = side;
                depth = side;
                cubeOrigin = new Vector3D(width / 2, height / 2, depth / 2);
                InitializeCube();
            }

            public Cube(int side, Vector3D origin)
            {
                width = side;
                height = side;
                depth = side;
                cubeOrigin = origin;

                InitializeCube();
            }

            public Cube(int Width, int Height, int Depth)
            {
                width = Width;
                height = Height;
                depth = Depth;
                cubeOrigin = new Vector3D(width / 2, height / 2, depth / 2);

                InitializeCube();
            }

            public Cube(int Width, int Height, int Depth, Vector3D origin)
            {
                width = Width;
                height = Height;
                depth = Depth;
                cubeOrigin = origin;

                InitializeCube();
            }

            #endregion
        }
    }
}