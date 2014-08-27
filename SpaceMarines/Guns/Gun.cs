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
using System.Collections.Generic;
using SpaceMarines.Utility;
using System.Linq;
namespace SpaceMarines
{
	public enum GunEnum { Infinite, Pickup }
    public abstract class Gun : ILevelUp
    {
        public Gun() 
        {
            UpgradeSchedule = CreateUpgradeSchedule();
            Level = 0;
			if (FireType == GunEnum.Infinite)
				FireCount = int.MaxValue;
            FireCountReachedZero += (x, y) =>
                {
                    Game.Player.Color = new DefaultColor();
                    Game.Player.RemoteUpdate();
                };
        }

        public TimeSpan CurrentFireRate;
        public DateTime LastFireTime = DateTime.MinValue;
        public bool CanFire
        {
            get { return DateTime.Now - LastFireTime > CurrentFireRate && FireCount > 0; }
        }
        public List<Bullet> Fire()
        {
            if (CanFire)
            {
                LastFireTime = DateTime.Now;
                List<Bullet> bullets = Upgrade.Fire();
                Remote.Send(Command.EntityCreateMany, bullets.OfType<Entity>().ToList()); 
				return bullets;
            }
            throw new Exception("Cannot fire.");
        }
		public abstract GunEnum FireType { get; }
		public int FireCount;
		public event EventHandler<EventArgs> FireCountReachedZero;
		public void OnFireCountReachedZero() { if (FireCountReachedZero != null) FireCountReachedZero(this, null); }

        public abstract TimeSpan BaseFireRate { get; }
        public abstract double BaseDamage { get; }
        public abstract double BaseSpeed { get; }
        protected abstract List<GunUpgrade> CreateUpgradeSchedule();
        public List<GunUpgrade> UpgradeSchedule;
        private int _Level;
        public GunUpgrade Upgrade;
        public int Level
        {
            get { return _Level; }
            set 
            {
                value = Math.Max(0,value);
                _Level = value;
                Upgrade = FetchScheduledUpgrade(_Level);
                CurrentFireRate = Upgrade.FireRate;
            }
        }
    
        public GunUpgrade FetchScheduledUpgrade(int targetLevel)
        {
            GunUpgrade current = null;
            int level = targetLevel;
            for (int i=0; i<UpgradeSchedule.Count && level >= 0; ++i)
            {
                current = UpgradeSchedule[i];
                level -= current.Levels;
            }
            current.StartLevel = targetLevel-(level+current.Levels);
            return current;
        }
    }

    public class GunUpgrade
    {
        public int StartLevel;
        public Func<int, double> SpeedModifierByLevel;
        public Func<int, double> DamageModifierByLevel;
        public Func<int, double> FireRateModifierByLevel;
        public int Levels = 1;
        public double SpeedModifier = 1; 
        public double DamageModifier = 1; 
        public double FireRateModifier = 1; 
        Gun Gun;
        private List<GunFire> GunFire;
        public double Speed { get { return Constants.BoardDiagonal * Gun.BaseSpeed * (SpeedModifierByLevel != null ? SpeedModifierByLevel(Game.Player.Level-StartLevel) : SpeedModifier); } }
        public double Damage { get { return Math.Min(100, Gun.BaseDamage * (DamageModifierByLevel != null ? DamageModifierByLevel(Game.Player.Level-StartLevel) : DamageModifier)); }}
        public TimeSpan FireRate { get { return TimeSpan.FromMilliseconds(Gun.BaseFireRate.TotalMilliseconds * (FireRateModifierByLevel != null ? FireRateModifierByLevel(Game.Player.Level-StartLevel) : FireRateModifier)); }}
        public GunUpgrade(Gun gun, GunFire gunFire) { Gun = gun; GunFire = new List<GunFire>() { gunFire }; }
        public GunUpgrade(Gun gun, params GunFire[] gunFire) { Gun = gun; GunFire = gunFire.ToList(); }

        public List<Bullet> Fire()
        {
            List<Bullet> bullets = new List<Bullet>();
			if (Gun.FireCount == 0) throw new Exception("Cannot fire because no bullets left.");
			if (Gun.FireCount != int.MaxValue) 
				--Gun.FireCount;
			if (Gun.FireCount == 0)
				Gun.OnFireCountReachedZero();
            
			foreach (GunFire fire in GunFire)
            {
                bullets.AddRange(fire.Fire(this));
            }
            return bullets;
        }

        public static Func<int, double> ModifierGenerator(double increaseRate, double increaseTo)
        {
            --increaseTo;
            double decayRate = 1 - increaseRate;
            return x => 1 + (increaseTo - increaseTo * Math.Pow(decayRate, x));
        }

    }

}
