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
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SpaceMarines
{
    [DataContract]
	public abstract class Buff : Pickup
	{
		public Buff(Ray ray, EntityColor color = null) : base(ray, color) { }

		public override void Apply(Player player)
		{
			player.Buffs.Where(buff => buff.GetType() == GetType()).ToList().ForEach(buff => player.Buffs.Remove(buff));
			player.Buffs.Add(this);
			ApplyBuff(player);
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = BuffDuration;
			timer.Tick += (sender, args) =>
			{
				timer.Stop();
				if (player.Buffs.Contains(this))
					DeBuff(player);
			};
			timer.Start();
		}

		protected abstract void ApplyBuff(Player player);
		protected abstract void DeBuff(Player player);
		public abstract TimeSpan BuffDuration { get; }
	}
}
