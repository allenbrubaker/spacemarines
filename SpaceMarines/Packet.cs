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
using System.Collections.Generic;

namespace SpaceMarines.Utility
{
	[KnownType(typeof(DefaultColor))]
	[KnownType(typeof(CustomColor))]
    [KnownType(typeof(AutoColor))]
	[KnownType(typeof(DamageAutoColor))]
    [KnownType(typeof(CommonBullet))]
	[KnownType(typeof(SmallBullet))]
	[KnownType(typeof(BombBullet))]
	[KnownType(typeof(StaminaPickup))]
	[KnownType(typeof(HealthPickup))]
	[KnownType(typeof(InvisiblePickup))]
	[KnownType(typeof(InvinciblePickup))]
	[KnownType(typeof(BombPickup))]
	[KnownType(typeof(LevelPickup))]
	[KnownType(typeof(Player))]
    [KnownType(typeof(List<Entity>))]
	public class Packet
	{
		public Command Command;
		public Object Object;
        public Packet() { } // Serializer needs a default parameterless object.
		public Packet(Command command, Object obj = null)
		{
			Command = command; Object = obj;
		}
	}
}
