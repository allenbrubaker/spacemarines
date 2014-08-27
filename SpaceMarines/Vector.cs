using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SpaceMarines.Utility;
using System.Runtime.Serialization;
namespace SpaceMarines
{
    [DataContract]
    public class Vector
    {
        [DataMember]public Point Direction;
        /// <param name="magnitude">Velocity: Percent of the board diagonal to move per second</param>
        public double Magnitude
        {
            get { return Length; }
            set { Direction = (UnitVector * value).Direction; }
        }

        public double X { get { return Direction.X; } set { Direction.X = value; }}
        public double Y { get { return Direction.Y; } set { Direction.Y = value; } }

        /// <param name="magnitude">Velocity: Percent of the board diagonal to move per second.  Defaulted to the magnitude determined from start and end point.</param>
        public Vector(Point p, double? magnitude = null)
        {
            Direction = p;
            if (magnitude.HasValue)
            {
                Magnitude = magnitude.Value;
            }

        }
		public Vector() { }
        /// <param name="magnitude">Velocity: Percent of the board diagonal to move per second; Defaulted to the magnitude determined from start and end point.</param>
        public Vector(double x, double y, double? magnitude=null) : this(new Point(x, y), magnitude) { }

        /// <param name="magnitude">Velocity: Percent of the board diagonal to move per second; Defaulted to the magnitude determined from start and end point.</param>
        public Vector(Point start, Point end, double? magnitude=null) : this(new Point(end.X-start.X, end.Y-start.Y), magnitude) {}

        public Vector UnitVector { get { return Length == 0 ? new Vector(0,0) : this / Length; } }
        public double Length { get { return X == 0 && Y == 0 ? 0 : Math.Sqrt(Math.Pow(Direction.X, 2) + Math.Pow(Direction.Y, 2)); } }

        public Vector Rotate(double degrees)
        {
            double rad = degrees * 2 * Math.PI / 360.0;
            return new Vector(X * Math.Cos(rad) - Y * Math.Sin(rad), X * Math.Sin(rad) + Y * Math.Cos(rad));
        }

        public Vector Normal(double magnitude=1.0)
        {
            Vector v = Rotate(90);
            v.Magnitude = magnitude;
            return v;
        }

        public static Vector operator -(Vector v) { return -1 * v; }
        public static Vector operator -(Vector u, Vector v) { return u + -v; }
        public static Vector operator +(Vector u, Vector v) { return new Vector(u.X + v.X, u.Y + v.Y); }
        public static Vector operator *(Vector v, double a) { return new Vector(v.X * a, v.Y * a); }
        public static Vector operator *(double a, Vector v) { return v * a; }
        public static Vector operator /(double a, Vector v) { return new Vector(a / v.X, a / v.Y); }
        public static Vector operator /(Vector v, double a) { return v * (1/a); }
    }
}
