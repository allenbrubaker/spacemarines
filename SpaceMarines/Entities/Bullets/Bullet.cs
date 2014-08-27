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
    public abstract class Bullet : Entity
    {
        public Bullet(Ray ray, double damage, EntityColor color=null) : base(ray, color) 
        {
            Log.GuiThread(() => Canvas.SetZIndex(Control, -1));
            Damage = damage;
        }
        [OnDeserializing]
        public void OnDeserializing(StreamingContext context) { Log.GuiThread(() => Canvas.SetZIndex(Control, -1)); } 

		public override EntityColor NullColor { get { return new DamageAutoColor(); } }
        public override bool UseDeadReckoning { get { return false; } }
        double _Damage;
        [DataMember]
        public double Damage 
        { 
            get { return _Damage; } 
            set 
            { 
                _Damage = value;             
                if (Color is DamageAutoColor)
                    UpdateAutoColor(Damage);
            }
        }
    }
}
