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
    public class Movement
    {
        DateTime LastMoved;
        Interpolate Interpolate;
        public Point Position;
        [DataMember] public Ray Ray;
        Ray Virtual;
        public Entity Entity;
        public Movement(Ray ray, Entity entity)
        {
            Entity = entity;
            Ray = ray;
            if (Entity.IsOwner && Entity.UseDeadReckoning)
                Virtual = Ray.DeepCopy;
            Position = ray.Position;
            Log.GuiThread(()=>Entity.SetCanvasPosition(Position));
            LastMoved = DateTime.Now;
        }

        public void UpdateVector(Vector v) { Ray.Vector = v; }

        public void InterpolateTo(Ray update)
        {
            if (Entity.IsOwner) return;
            if (Interpolate == null)
                Interpolate = new Spline(Ray, update);
            else
                Interpolate.InterpolateTo(update);
        }

        public void Move()
        {
            if (Interpolate != null)
            {
                Interpolate.Move();
                Position = Interpolate.Position;
            }
            else
            {
                double t = DateTime.Now.Subtract(LastMoved).TotalSeconds;
                Ray.Move(t);
                if (Virtual != null) Virtual.Move(t);
                HandleDeadReckoning();
                Position = Ray.Position;
            }

            Entity.SetCanvasPosition(Position);
            LastMoved = DateTime.Now;
        }

        public void Move(Point p)
        {
            if (!Entity.IsOwner) return;
            Ray.Position = p;
            Position = p;
            HandleDeadReckoning();
        }

        void HandleDeadReckoning()
        {
            if (Virtual != null)
            {
                if ((new Vector(Virtual.Position, Ray.Position)).Length > Constants.DeadReckoningThreshhold)
                {
                    Entity.RemoteUpdate();
                    Virtual = Ray.DeepCopy;
                }
            }
        }

    }
}
