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
using System.Reflection;
using System.Linq;
using System.Windows.Threading;
using System.Runtime.Serialization;

namespace SpaceMarines
{
    [DataContract]
	public abstract class Pickup : Entity
	{
		public Pickup(Ray ray=null, EntityColor color = null) : base(ray, color) 
        {
            Log.GuiThread(() => Canvas.SetZIndex(Control, -2));
        }

        [OnDeserializing]
        public void OnDeserializing(StreamingContext context) { Log.GuiThread(() => Canvas.SetZIndex(Control, -2)); }

		public override bool UseDeadReckoning
		{
			get { return false; }
		}
		public virtual TimeSpan DropRate { get { return TimeSpan.FromSeconds(Constants.PickupDropRate.TotalSeconds * ( 1 + ((Constants.Random.NextDouble() * 2 * Constants.PickupDropRateRandomizer) - Constants.PickupDropRateRandomizer / 2.0))); } }
	
		public static Pickup RandomPickup 
		{
			get
			{
                return (Pickup)Activator.CreateInstance(PickupTypes[Constants.Random.Next(PickupTypes.Length)], null, null);
			}
		}

		public abstract void Apply(Player player);

		private static Type[] _pickupTypes;
		private static Type[] PickupTypes
		{
			get
			{
				if (_pickupTypes == null)
				{
					Type type = typeof(Pickup);
					Type[] pickups = Assembly.GetExecutingAssembly().GetTypes().Where(t => type.IsAssignableFrom(t) && !t.IsAbstract).ToArray();
					_pickupTypes = pickups;

				}
				return _pickupTypes;
			}
		}
	}
}
