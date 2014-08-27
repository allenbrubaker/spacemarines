using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using SpaceMarines.Utility;
using System.Collections.Generic;

namespace SpaceMarines
{
	public static class Remote
	{
        static Byte[] Buffer = new Byte[100000];
        private static Socket Socket;

        public static bool IsConnected { get { return Socket != null && Socket.Connected; } }

        static Remote()
        {
            Log.GuiThread(()=>App.Current.Exit += (x, y) => Close());
        }

        /// <summary>
        /// Connect to host and populate Socket field. Returns when connection is established.
        /// </summary>
        public static bool Connect(out int clientID, out bool isCoordinator)
        {
			DnsEndPoint hostAddress = new DnsEndPoint(Constants.ServerAddress, Constants.GamePort);
			Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            AutoResetEvent mutex = new AutoResetEvent(false);
            var args = new SocketAsyncEventArgs { RemoteEndPoint = hostAddress };
            args.Completed += (x, y) =>  mutex.Set();
            Socket.ConnectAsync(args);
            mutex.WaitOne(); // Make this function blocking
            
            Log.ToggleConnectStatus(Socket.Connected);
            
            if (!IsConnected)
            {
                clientID = 0; isCoordinator = false;
                return false;
            }
            
            DateTime timer = DateTime.Now;
            string[] response = (Remote.Receive()[0].Object as String).Split('.'); // Need to populate clientID before doing anything else.
            Log.UpdateLatency((DateTime.Now - timer).TotalMilliseconds);
            clientID = int.Parse(response[0]);
            isCoordinator = bool.Parse(response[1]);
            return true;
        }

        public static void Send(string sendMessage)
        {
            Send(Command.LogMessage, "[client" + Game.ClientID + "]" + sendMessage);
        }

		public static void Send(Command c, Object o=null)
		{
			Send(new Packet(c,o));
		}

        public static void Send(Packet p)
        {
            if (!IsConnected)
                return;
            //byte[] data = Marshal.Pack(p);
            byte[] data = AddSizeHeader(Marshal.Pack(p));
            var args = new SocketAsyncEventArgs();
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
		/// Blocking receive.
		/// </summary>
		/// <param name="action"></param>
		public static List<Packet> Receive()
		{
            List<Packet> packets = new List<Packet>();
			var args = new SocketAsyncEventArgs();
            Byte[] data;
			args.SetBuffer(Remote.Buffer, 0, Remote.Buffer.Length);
			AutoResetEvent mutex = new AutoResetEvent(false);
			args.Completed += (x, y) =>
				{
                    if (y.BytesTransferred == 0)
                        packets.Add(new Packet(Command.ServerDisconnect));
                    else
                    {
                        for (int i=0, size; i < y.BytesTransferred;)
                        {
                            size = (y.Buffer[i] << 8) | y.Buffer[i + 1];
                            i += 2;
                            data = new byte[size];
                            Array.Copy(y.Buffer, i, data, 0, size);
                            i += size;
                            packets.Add(Marshal.Unpack<Packet>(data));
                        }
                    }
                    mutex.Set();
				};
			Socket.ReceiveAsync(args); // Not sure if eventargs need to be populated with something?
			mutex.WaitOne();
			return packets;
		}

		public static void Close()
		{
			if (Socket.Connected)
				Socket.Shutdown(SocketShutdown.Both); // Sends 0 byte packet to server to signal connection is closing.  If this is not called, an exception is called server side on the socket.receive()
		}


	}
}
