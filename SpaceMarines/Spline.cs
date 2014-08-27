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

namespace SpaceMarines
{

    public abstract class Interpolate
    {
        protected DateTime StartTime = DateTime.Now;
        protected Ray Start;
        protected Ray Actual; // True position/vector of actual remote entity that we need to eventually smoothly align with
        public Point Position;
        public double SplineTime;
        public Interpolate(Ray start, Ray actual)
        {
            Start = start;
            Actual = actual;
            Position = Start.Position;
        }

        public abstract void InterpolateTo(Ray ray);
        public abstract void Move();
    }

    public class LinearInterpolation : Interpolate
    {
        public LinearInterpolation(Ray start, Ray actual) : base(start, actual) { InterpolateTo(actual); }
        public override void InterpolateTo(Ray ray)
        {
            StartTime = DateTime.Now;
            Point target = ray.PointAt(Constants.SplineTime);
            Start = new Ray(Position, target);
            Actual = ray;
        }
        public override void Move()
        {
            double t = DateTime.Now.Subtract(StartTime).TotalSeconds;
            double splineT = t / Constants.SplineTime;
            if (splineT < 1)
                Position = Start.PointAt(splineT);
            else
                Position = Actual.PointAt(t);
        }
    }


    public class Spline : Interpolate
    {
        double[] Coefficients = new double[8];
        Point[] Controls = new Point[4];

        public Spline(Ray start, Ray actual) : base(start,actual)
        {
            UpdateCoefficients();
        }

        public void UpdateCoefficients()
        {
            double vectorInfluence = Constants.SplineIntermediateControlInfluence/2.0;
            double x0,x1,x2,x3,y0,y1,y2,y3;
            Controls[0].X = x0 = Start.Position.X;
            Controls[0].Y = y0 = Start.Position.Y;
            Controls[3].X = x3 = Actual.Position.X + Actual.Vector.X * Constants.SplineTime;
            Controls[3].Y = y3 = Actual.Position.Y + Actual.Vector.Y * Constants.SplineTime; // Calculate projected point that the packet data point will be in.
            double distance = Math.Sqrt((x0-x3)*(x0-x3) + (y0-y3)*(y0-y3));
            Controls[1].X = x1 = x0 + (Start.Vector.Magnitude == 0.0 ? 0 : Start.Vector.X / Start.Vector.Magnitude * vectorInfluence * distance);
            Controls[1].Y = y1 = y0 + (Start.Vector.Magnitude == 0.0 ? 0 : Start.Vector.Y / Start.Vector.Magnitude * vectorInfluence * distance);
            
            Controls[2].X = x2 = x3 - (Actual.Vector.Magnitude == 0.0 ? 0 :Actual.Vector.X / Actual.Vector.Magnitude * vectorInfluence * distance);
            Controls[2].Y = y2 = y3 - (Actual.Vector.Magnitude == 0.0 ? 0 :Actual.Vector.Y / Actual.Vector.Magnitude * vectorInfluence * distance);
            Coefficients[0] = x3 - 3 * x2 + 3 * x1 - x0;
            Coefficients[1] = 3 * x2 - 6 * x1 + 3 * x0;
            Coefficients[2] = 3 * x1 - 3 * x0;
            Coefficients[3] = x0;
            Coefficients[4] = y3 - 3 * y2 + 3 * y1 - y0;
            Coefficients[5] = 3 * y2 - 6 * y1 + 3 * y0;
            Coefficients[6] = 3 * y1 - 3 * y0;
            Coefficients[7] = y0;
        }

        public override void InterpolateTo(Ray actual)
        {
            double t = DateTime.Now.Subtract(StartTime).TotalSeconds;
            double splineT = t / Constants.SplineTime; // Interpolation always happens from 0 -> 1 (startpoint to endpoint)
            StartTime = DateTime.Now;
            if (splineT < 1)
            {
                // Calculate current ray from the current position
                Func<Point, Point, Point> f = (a, b) => new Point(a.X + splineT * (b.X - a.X), a.Y + splineT * (b.Y - a.Y));
                Point p1 = f(Controls[0], Controls[1]);
                Point p2 = f(Controls[1], Controls[2]);
                Point p3 = f(Controls[2], Controls[3]);
                Point p4 = f(p1, p2);
                Point p5 = f(p2, p3);
                Vector v = new Vector(p4, p5, actual.Vector.Magnitude);
                Start = new Ray(Position, v);
            }
            else
                Start = new Ray(Position, Actual.Vector);
            Actual = actual;
            UpdateCoefficients();
        }

        public override void Move()
        {
            double t = DateTime.Now.Subtract(StartTime).TotalSeconds;
            double splineT = t / Constants.SplineTime;
            if (splineT < 1)
            {
                Position.X = ((Coefficients[0] * splineT + Coefficients[1]) * splineT + Coefficients[2]) * splineT + Coefficients[3];
                Position.Y = ((Coefficients[4] * splineT + Coefficients[5]) * splineT + Coefficients[6]) * splineT + Coefficients[7];
            }
            else
            {
                Position.X = Actual.Position.X + Actual.Vector.X * t;
                Position.Y = Actual.Position.Y + Actual.Vector.Y * t;
            }
        }

    }
}
