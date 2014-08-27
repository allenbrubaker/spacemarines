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
    public class Server
    {
        private Socket ServerSocket;
        private Thread ListenerThread;
        private List<Client> Clients = new List<Client>();
        private Handler Handler;
		public MainWindow Window = new MainWindow();
		private int clientID = 0;
		public Client Coordinator;
        public static List<MainWindow> AllServerWindows = new List<MainWindow>();

        public Server(Handler handler, int port)
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, port)); 
            Handler = handler;
            App.Current.Exit += (x,y) => Shutdown();
            Window.IsVisibleChanged += (x,y) => 
                { 
                    if (!AllServerWindows.Aggregate(false, (acc,window) => acc || window.IsVisible)) 
                        App.Current.Shutdown(); 
                };
			Window.Title = handler.GetType().Name.ToString();
            AllServerWindows.Add(Window);
        }

        public void Start(bool showWindow=true)
        {
            if (showWindow) Window.Show();
            ServerSocket.Listen((int)SocketOptionName.MaxConnections); //Non-blocking
            ListenerThread = new Thread(new ThreadStart(Listen));
            ListenerThread.Start();
        }
        
        /// <summary>
        /// Spawns a new thread who listens for client requests indefinitely.
        /// </summary>
        private void Listen()
        {
			while (true)
			{
				Socket socket = ServerSocket.Accept(); // Blocking
				Client client = new Client(++clientID, socket, this, Handler);
				lock (Clients)
				{
					if (Coordinator == null)
					{
						Coordinator = client;
					}
					Clients.Add(client);
				}
				UpdateClientCount();

				client.Start(); // Spawns new thread to handle listening on this client

			}
        }


        public void Remove(Client client)
        {
            lock (Clients)
            {
                Clients.Remove(client);
				if (!IsServerDisconnecting && client == Coordinator)
				{
					Coordinator = Clients.Count == 0 ? null : Clients.Aggregate((acc, c) => acc = (c.ID < acc.ID) ? c : acc);
					if (Coordinator != null)
						Coordinator.Remote.Send(new Packet(Command.ClientElected), true);
				}
            }
			UpdateClientCount();
			
        }
		public bool IsServerDisconnecting;
        void Shutdown()
        {
			IsServerDisconnecting = true;
            //ServerSocket.Shutdown(SocketShutdown.Both);
            ServerSocket.Close();
            ListenerThread.Abort(); // Must close socket first before aborting thread or else it'll wait forever for thread to accept.
            for (int i = Clients.Count - 1; i >= 0; --i)
            {
                Clients[i].Dispose(); // Remember client calls Remove(..) function, which removes the client from the list.  (Therefore don't use foreach loop or remove from beginning)
            }
        }

		void UpdateClientCount()
		{
			Window.Dispatcher.BeginInvoke(new Action(() => Window.tbClientCount.Content = Clients.Count));
		}

        public void Broadcast(int sourceClientID, Packet p, bool includeSizeHeader)
        {
            Broadcast(sourceClientID, Marshal.Pack(p), includeSizeHeader);
        }
        public void Broadcast(int sourceClientID, Byte[] data, bool includeSizeHeader)
        {
            lock (Clients)
            {
                foreach (Client c in Clients)
                {
                    if (c.ID != sourceClientID)
                        c.Remote.Send(data, includeSizeHeader);
                }
            }
        }

    }


    


}