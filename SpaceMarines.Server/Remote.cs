using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using SpaceMarines.Utility;

namespace SpaceMarines.Server
{
	public class Remote
	{
        Byte[] Buffer = new Byte[100000];
        public Socket Socket;
        public Remote(Socket socket){ Socket = socket; }
		
		public void Send(Packet packet, bool includeSizeHeader) 
		{
			Send(Marshal.Pack(packet), includeSizeHeader);
		}

		public void Send(byte[] data, bool includeSizeHeader)
		{
			var args = new SocketAsyncEventArgs();
            if (includeSizeHeader)
                data = AddSizeHeader(data);
			args.SetBuffer(data, 0, data.Length);
			Socket.SendAsync(args);
		}

        public static byte[] AddSizeHeader(byte[] data)
        {
            byte[] x = new byte[data.Length + 2];
            x[1] = (byte)((data.Length & 0x000000FF));
            x[0] = (byte)((data.Length & 0x0000FF00) >> 8);
            data.CopyTo(x, 2);
            return x;
        }

        /// <summary>
        /// Blocking Receive
        /// </summary>
		public Byte[] Receive()
		{
            Byte[] data; // There should be a more exact way to tell what the size of data being sent is.
            //try
            //{
                int size = Socket.Receive(Buffer);
                data = new Byte[size];
                for (int i = 0; i < size; ++i) data[i] = Buffer[i];
                if (size == 0) // If you are using a connection-oriented Socket, the Receive method will read as much data as is available, up to the size of the buffer. If the remote host shuts down the Socket connection with the Shutdown method, and all available data has been received, the Receive method will complete immediately and return zero bytes.
                    return null;
            //}
            //catch (Exception e) { return null; }
			return data;
		}

		public void Close()
		{
			if (Socket.Connected)
				Socket.Close();
		}
	}
}
