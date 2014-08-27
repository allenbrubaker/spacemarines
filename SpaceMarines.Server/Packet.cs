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
