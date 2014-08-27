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
using System.Collections.Generic;
namespace SpaceMarines
{
    public class MachineGun : Gun
    {
        public override TimeSpan BaseFireRate { get { return Constants.MachineGunFireRate; } }
        public override double BaseSpeed { get { return Constants.MachineGunVelocity; } }
        public override double BaseDamage { get { return Constants.MachineGunDamage; } }

        protected override List<GunUpgrade> CreateUpgradeSchedule()
        {
            //return new List<GunUpgrade>() { new GunUpgrade(this, new ArcShot<CommonBullet>(10, 25)) };
            return new List<GunUpgrade>() 
            { 
                new GunUpgrade(this, new SingleShot<CommonBullet>()),
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 60)),
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 30)),
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 15)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 60)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 30)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 15)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 60)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 30)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 15)),
                new GunUpgrade(this, new ParallelShot<CommonBullet>(4, .03)),
                
                new GunUpgrade(this, new SingleShot<CommonBullet>()) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 60)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 30)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 15)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 60)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 30)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 15)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 60)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 30)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 15)) { SpeedModifier=1.5 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(4, .03)) { SpeedModifier=1.5 },

                new GunUpgrade(this, new SingleShot<CommonBullet>()) { SpeedModifier=3 },
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 60)) { SpeedModifier=3 },
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 30)) { SpeedModifier=3 },
                new GunUpgrade(this, new ArcShot<CommonBullet>(3, 15)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 60)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 30)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(2, .015), new ArcShot<CommonBullet>(2, 15)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 60)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 30)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(3, .02), new ArcShot<CommonBullet>(2, 15)) { SpeedModifier=3 },
                new GunUpgrade(this, new ParallelShot<CommonBullet>(4, .03)) { SpeedModifier=3 },
            };
        }

		public override GunEnum FireType
		{
			get { return GunEnum.Infinite; }
		}
    }
}
