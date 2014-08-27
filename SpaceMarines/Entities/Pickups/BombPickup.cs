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
using System.Runtime.Serialization;
namespace SpaceMarines
{
    [DataContract]
	public class BombPickup : Pickup
	{
		public BombPickup(Ray ray, EntityColor color = null) : base(ray, color) { }

		public override void Apply(Player player)
		{
			Bomber b = Game.Guns.Values.FirstOrDefault(e => e is Bomber) as Bomber;
			if (b != null)
			{
				++b.FireCount;
				player.Color = new CustomColor(Colors.Black);
                player.RemoteUpdate();
			}
		}
	}
}


