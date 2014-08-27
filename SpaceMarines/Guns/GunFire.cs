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
using System.Linq;
using System.Collections.Generic;
using SpaceMarines.Utility;

namespace SpaceMarines
{
    public abstract class GunFire
    {
		public EntityColor Color;
        public abstract List<Bullet> Fire(GunUpgrade upgrade);
    }

    public class SingleShot<T> : GunFire where T : Bullet
    {
        public double Heading, Offset;
		public SingleShot(double heading = 0, double offset = 0, EntityColor color = null) { Color = color; Heading = heading; Offset = offset; }
        public override List<Bullet> Fire(GunUpgrade upgrade)
        {
            Vector offset, v = new Vector(Game.Player.Position, Game.MousePosition, upgrade.Speed);
            if (Heading != 0) v = v.Rotate(Heading);
            if (Offset != 0)
                offset = v.Normal(Offset);
            else
                offset = new Vector(0, 0);
            
            return new List<Bullet>() { (Bullet)Activator.CreateInstance(typeof(T), new Ray(new Point(Game.Player.Position.X + offset.X, Game.Player.Position.Y + offset.Y), v), upgrade.Damage, Color) };
        }
    }
    public class ArcShot<T> : GunFire where T : Bullet
    {
        public double Angle;
        public int Bullets;
		public ArcShot(int bullets, double angle, EntityColor color = null) { Color = color; Angle = angle; Bullets = bullets; }
        public override List<Bullet> Fire(GunUpgrade upgrade)
        {
            List<Bullet> list = new List<Bullet>();
            double inc = Angle / (Bullets-1), start = Angle / 2;
            for (double heading = start; heading >= -start; heading -= inc)
            {
                list.AddRange((new SingleShot<T>(heading:heading, color:Color)).Fire(upgrade));
            }
            return list;
        }
    }
    public class ParallelShot<T> : GunFire where T : Bullet
    {
        public double Width;
        public int Bullets;

        /// <param name="bullets"></param>
        /// <param name="width">Width is a value from 0-1 and denotes percent of board diagonal.</param>
        /// <param name="color"></param>
		public ParallelShot(int bullets, double width, EntityColor color = null) { Color = color; Bullets = bullets; Width = width * Constants.BoardDiagonal ; }
        public override List<Bullet> Fire(GunUpgrade upgrade)
        {
            List<Bullet> list = new List<Bullet>();
            double start = Width / 2, inc = Width / (Bullets-1);
            for (double offset = start; offset >= -start; offset -= inc)
            {
                list.AddRange((new SingleShot<T>(offset: offset, color:Color)).Fire(upgrade));
            }
            return list;
        }
    }
}
