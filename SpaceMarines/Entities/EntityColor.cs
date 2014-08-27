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
using System.Xml.Linq;
using System.Runtime.Serialization;
namespace SpaceMarines
{
    [DataContract]
    public abstract class EntityColor
    {
        /// <param name="hue">Hue from 0..360 degrees.</param>
        /// <param name="saturation">Saturation from 0..1</param>
        /// <param name="value">Brightness or value from 0..1</param>
        private Color FromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            var v = Convert.ToByte(value);
            var p = Convert.ToByte(value * (1 - saturation));
            var q = Convert.ToByte(value * (1 - f * saturation));
            var t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public Color FromHex(string hexColor)
        {
                int a,r,g,b=0;
                hexColor = hexColor.Replace("#","");
                if (hexColor.Length != 6 && hexColor.Length != 8)
                    throw new Exception("Improperly formatted color hex string.");
                a = int.Parse(hexColor.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
                r = int.Parse(hexColor.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
                g = int.Parse(hexColor.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
                if (hexColor.Length == 8)
                {
                    b = int.Parse(hexColor.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
                    return Color.FromArgb((byte)a,(byte)r,(byte)g,(byte)b);
                }
                else
                    return Color.FromArgb(255, (byte)a, (byte)r, (byte)g);
        }

        protected Color CalculateColorByProgress(double progress, Tuple<int, int> hueRange, double saturation, double brightness)
        {
			progress = Math.Min(1.0, progress);
            double distance = ((hueRange.Item2 - hueRange.Item1) % 360 + 360) % 360;
            double hue = (((progress * distance) + hueRange.Item1) % 360 + 360) % 360;
            return FromHSV(hue, saturation, brightness);
        }

    }

    [DataContract]
    public class DefaultColor : EntityColor { }
		
    [DataContract]
    public class CustomColor : EntityColor
    {
        [DataMember] 
        public Color Color;
        public CustomColor(Color c)
        {
            Color = c;
        }
        public CustomColor(string hexColor)
        {
            Color = FromHex(hexColor);
        }

    }
    
    [DataContract]
    public abstract class AutoColor : EntityColor
    {
        public abstract Color Update(double value);
    }

    [DataContract]
    public class DamageAutoColor : AutoColor
    {
        public override Color Update(double damage)
        {
            return CalculateColorByProgress(damage/.7 - .05, Constants.DamageColorRange, Constants.Saturation, Constants.Brightness);
        }
    }


}
