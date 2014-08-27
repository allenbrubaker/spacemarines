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
using System.Xml.Linq;
using System.Runtime.Serialization;
namespace SpaceMarines
{
    [DataContract]
    public class Ray 
    {
        [DataMember]public Point Position;
        [DataMember]public Vector Vector;

		public Ray()
		{
			Vector = new Vector(0, 0);
		}

        public Ray(Point position, Vector vector)
        {
            Position = position;
            Vector = vector;
        }

        /// <param name="magnitude">Velocity: Percent of the board diagonal to move per second</param>
        public Ray(Point start, Point end, double? magnitude=null, double rotateDegrees=0)
        {
            Position = start;
            Vector = (new Vector(start, end, magnitude));
            if (rotateDegrees != 0)
                Vector = Vector.Rotate(rotateDegrees);
        }

        public void Move(double t)
        {
            Position.X += t * Vector.X;
            Position.Y += t * Vector.Y;
        }

        public void Move(Point to)
        {
            Position = to;
        }

        public Point PointAt(double t)
        {
            return new Point(Position.X + Vector.X * t, Position.Y + Vector.Y * t);
        }

        public Ray DeepCopy
        {
            get
            {
                return new Ray(Position, new Vector(Vector.Direction)); // remember Point is a struct so it always does a copy of itself.
            }
        }

    }
}
