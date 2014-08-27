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
using System.Runtime.Serialization;

namespace SpaceMarines
{
    [DataContract]
	public class HealthPickup : Pickup
	{
		public HealthPickup(Ray ray, EntityColor color = null) : base(ray, color) { }

		public override void Apply(Player player)
		{
			player.Health = 1.0;
            player.RemoteUpdate();
		}
	}
}
