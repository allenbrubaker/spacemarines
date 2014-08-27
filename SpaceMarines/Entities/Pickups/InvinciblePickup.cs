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
	public class InvinciblePickup : Buff
	{
		public InvinciblePickup(Ray ray, EntityColor color = null) : base(ray, color) { }

		public override TimeSpan BuffDuration
		{
			get { return Constants.InvincibleDuration; }
		}

		protected override void ApplyBuff(Player player)
		{
			player.IsInvincible = true;
		}

		protected override void DeBuff(Player player)
		{
			player.IsInvincible = false;
		}
	}
}
