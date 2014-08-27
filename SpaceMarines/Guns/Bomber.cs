using System;
using SpaceMarines.Utility;
using System.Collections.Generic;
using System.Windows;

namespace SpaceMarines
{
    public class Bomber :Gun
    {
        public override TimeSpan BaseFireRate { get { return Constants.BomberFireRate; } }
        public override double BaseSpeed { get { return Constants.BomberVelocity; } }
        public override double BaseDamage { get { return Constants.BomberDamage; } }

        protected override List<GunUpgrade> CreateUpgradeSchedule()
        {
            return new List<GunUpgrade>() { new GunUpgrade(this, new SingleShot<BombBullet>(color:new DefaultColor())) };
        }

		public override GunEnum FireType
		{
			get { return GunEnum.Pickup; }
		}
    }
}
