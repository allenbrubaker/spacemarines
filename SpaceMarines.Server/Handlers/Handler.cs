using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Sockets;
using SpaceMarines.Utility;

namespace SpaceMarines.Server
{
    public abstract class Handler
    {
        public Client Client;
        public Server Server;

        public abstract void Connected();
        public abstract bool Receive(Byte[] p);
        public abstract void Closing();

        public void Log(string message) { Client.Log(message); }
        public void Send(Byte[] data, bool includeSizeHeader) { Client.Remote.Send(data, includeSizeHeader); }
        public void Send(Packet p, bool includeSizeHeader) { Client.Remote.Send(p, includeSizeHeader); }
        public void Broadcast(Packet p, bool includeSizeHeader) { Server.Broadcast(this.Client.ID, p, includeSizeHeader); }
        public void Broadcast(Byte[] data, bool includeSizeHeader) { Server.Broadcast(this.Client.ID, data, includeSizeHeader); }
		public bool IsCoordinator { get { return Server.Coordinator == Client; } }

    }
}