using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpaceMarines.Utility;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace SpaceMarines.Server
{
    public class GameHandler : Handler
    {
        public override void Connected()
        {
            Send(new Packet(Command.ClientConnect, Client.ID.ToString() + "." + IsCoordinator), true); // Very important to send immediately before any other broadcast messages can be sent to this client (or else entities get created on client without client knowing its client id!).
		}

        public override bool Receive(Byte[] data)
        {
            Broadcast(data, false);
			return true;
        }

        public override void Closing()
        {
            Broadcast(new Packet(Command.ClientDisconnect, Client.ID), true);
        }
    }
}