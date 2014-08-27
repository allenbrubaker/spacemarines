using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SpaceMarines.Utility;
using System.Runtime.Serialization;
namespace SpaceMarines
{
    [DataContract]
	public class Player : Entity, ILevelUp
	{
		public Player(Ray ray=null)
			: base(ray)
		{
            Level = 0;
		}

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Player original = Game.Players.Where(p => p.Equals(this)).FirstOrDefault();
            if (original == null) return;
            ucPlayer control = this.Control as ucPlayer;
            ucPlayer originalControl = original.Control as ucPlayer;
            Log.GuiThread(() => control.HealthAnimation.Seek(originalControl.HealthAnimation.GetCurrentTime()));
        }


		public override bool UseDeadReckoning { get { return true; } }
        [DataMember] public double Armor;
		double _Health = 1.0;
		[DataMember] 
        public double Health
		{
			get { return _Health; }
			set
			{
				_Health = value;
				Log.GuiThread(() => (Control as ucPlayer).SetHealthPercent(_Health));
			}
		}
		[DataMember] public bool IsInvincible;
		[DataMember] public bool IsHyper;

		public List<Buff> Buffs = new List<Buff>();

        bool _IsInvisible;
		[DataMember]
        public bool IsInvisible
		{
			get { return _IsInvisible; }
			set
			{
				_IsInvisible = value;
				Log.GuiThread(() => (Control as ucPlayer).ToggleInvisibility(value, IsOwner));
			}
		}
        [DataMember] public int? LastKilledBy;
        [DataMember] public int Kills;
        [DataMember] public int Deaths;
		int _Level;
		[DataMember]
        public int Level
		{
			get
			{
				return _Level;
			}
			set
			{
                value = Math.Max(value,0);
                _Level = value;
                Armor = 1-Math.Pow(1-Constants.PlayerArmorIncreaseRatePerLevel, value);
                if (this == Game.Player)
                {
                    foreach (Gun gun in Game.Guns.Values)
                        gun.Level = value;
                }
			}
		}

		public void Hit(Bullet b)
		{
			if (IsInvincible) return;
			Health -= b.Damage * (1 - Armor);
            if (IsDead)
                LastKilledBy = b.Owner;
		}

		public bool IsDead { get { return Health < .000001; } }

        public void Resurrect()
        {
            ++Deaths;
            Level = Math.Max(0,Level - 1);
            Health = 1.0;
            RandomMove();
            RemoteResurrect(); 
        }

        public void RemoteResurrect() { ToServer(Command.PlayerResurrect); }
	}
}
