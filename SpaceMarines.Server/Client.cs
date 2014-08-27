using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Sockets;
using SpaceMarines.Utility;
using System.Threading;

namespace SpaceMarines.Server
{
    public class Client
    {
        public int ID;
        public Socket Socket;
        public Remote Remote;
        public Server Server;
        public Handler Handler;
        public Thread Listener;

		public void Log(string message)
		{
			Server.Window.Log.WriteLine("[Client " + ID + "]  " + message);
		}

        public Client(int id, Socket clientSocket, Server server, Handler handler)
        {
			ID = id;
            Socket = clientSocket;
            Remote = new Remote(clientSocket);
            Handler = Activator.CreateInstance(handler.GetType()) as Handler;
            Handler.Client = this;
            Handler.Server = server;
            Server = server;
        }

        public void Start()
        {
            Listener = new Thread(new ThreadStart(Consume));
            Listener.Start();
        }

        private void Consume()
        {
			Log("joined");
            Handler.Connected();
			byte[] data;
            try
            {
				while ((data = Remote.Receive()) != null) // Loop until the remote host calls Shutdown(), which will receive 0 bytes and return null.
				{
					if (!Handler.Receive(data)) // We need to eventually use beginreceive() and spawn a new thread for each new message coming in.... keep it simple for now while we debug
						break;
				}
			}
			catch (Exception e) { Log("ERROR: " + e.Message); }
			Dispose();
        }

        public void Dispose()
        {
			Log("quit");
            Handler.Closing();
			Remote.Close();
            Server.Remove(this);
        }
    }
}