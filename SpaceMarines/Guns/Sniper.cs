using System;
using SpaceMarines.Utility;
using System.Collections.Generic;
using System.Windows;

namespace SpaceMarines
{
    public class Sniper :Gun
    {
        public override TimeSpan BaseFireRate { get { return Constants.SniperFireRate; } }
        public override double BaseSpeed { get { return Constants.SniperVelocity; } }
        public override double BaseDamage { get { return Constants.SniperDamage; } }

        protected override List<GunUpgrade> CreateUpgradeSchedule()
        {
            return new List<GunUpgrade>() 
            { 
                new GunUpgrade(this, new SingleShot<SmallBullet>()) { SpeedModifierByLevel = GunUpgrade.ModifierGenerator(.01, 4), DamageModifierByLevel = GunUpgrade.ModifierGenerator(.01, 3) } 
            } ;
        }

		public override GunEnum FireType
		{
			get { return GunEnum.Infinite; }
		}
    }
}
