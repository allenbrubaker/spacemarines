
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SpaceMarines.Utility;
using System.Runtime.Serialization;

namespace SpaceMarines.Utility
{
	public static class Marshal
	{
		public static byte[] Pack<T>(T data)
		{
            Byte[] serialized;
            using (MemoryStream stream = new MemoryStream())
            {
                (new DataContractSerializer(typeof(T))).WriteObject(stream, data);
                serialized = stream.ToArray();
            }
            return QuickLZ.compress(serialized, 1);
		}

		public static T Unpack<T>(byte[] data)
		{
            T value;
            using (MemoryStream stream = new MemoryStream(QuickLZ.decompress(data)))
                value = (T)(new DataContractSerializer(typeof(T))).ReadObject(stream);
			return value; 
		}
	}
}
