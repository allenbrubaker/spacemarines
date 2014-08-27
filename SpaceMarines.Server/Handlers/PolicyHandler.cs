using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpaceMarines.Utility;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SpaceMarines.Server
{
    public class PolicyHandler : Handler
    {
        public override void Connected()
        {
            //throw new NotImplementedException();
        }

        public override bool Receive(Byte[] data)
        {
            //System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
			string policyFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><access-policy><cross-domain-access><policy><allow-from><domain uri=\"*\"/></allow-from><grant-to><socket-resource port=\"4502-4534\" protocol=\"tcp\"/></grant-to></policy></cross-domain-access></access-policy>";
			Send(System.Text.Encoding.UTF8.GetBytes(policyFile), false);
			return false;
		}

        public override void Closing()
        {
            //throw new NotImplementedException();
        }
    }
}