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
using System.Threading;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace SpaceMarines
{
	public class GameListener
	{
		public Game Game;
        public Thread ListenerThread;

        /// <returns>True iff succesfully connected</returns>
		public void Listen()
		{
            ListenerThread = new Thread(new ThreadStart(DoListen));
            ListenerThread.Start();
		}

		private void DoListen()
		{
			while (true)
			{
                foreach (Packet packet in Remote.Receive())
                {
                    switch (packet.Command)
                    {
                        case Command.LogMessage: LogMessage((string)packet.Object); break;
                        case Command.EntityCreate: EntityCreate(packet.Object as Entity); break;
                        case Command.EntityUpdate: EntityUpdate(packet.Object as Entity); break;
                        case Command.EntityDestroy: EntityDestroy(packet.Object as Entity); break;
                        case Command.EntityCreateMany: EntityCreateMany((List<Entity>)packet.Object); break;
                        case Command.PlayerResurrect: PlayerResurrect(packet.Object as Player); break;
                        case Command.ClientElected: ClientElected(); break;
                        case Command.ClientDisconnect: ClientDisconnect((int)packet.Object); break;
                        case Command.ServerDisconnect: return;
                    }
                }

			}
		}

        private void LogMessage(string msg)
        {
            string pattern = @"\[client(\d*)\]|\[server\]";
            Match match = Regex.Match(msg, pattern,RegexOptions.IgnoreCase);
            if (match.Success)
                Log.WriteLine(Regex.Replace(msg, pattern, "", RegexOptions.IgnoreCase), match.Value.ToLower()=="[server]" ? -1 : int.Parse(match.Groups[1].Value));
            else
                Log.WriteLine(msg);
        }


        private void ClientDisconnect(int clientID)
        {
            Player p = Game.Players.Where(e => e.Owner == clientID).FirstOrDefault();
            if (p != null)
                Game.Remove(p);
        }

        /// <summary>
        /// Player died and was resurrected so we need to update kills (level) of the person who killed him!
        /// </summary>
        private void PlayerResurrect(Player player)
        {
            lock (Game.Entities)
            {
                var killer = Game.Players.Where(e => e.Owner == player.LastKilledBy).FirstOrDefault();
                if (killer != null)
                {
                    ++killer.Kills;
                    ++killer.Level;
                    killer.Health = 1.0;
                }
            }
            EntityUpdate(player);
        }

		private void ClientElected()
		{
			lock (Game.Entities)
			{
				Game.IsCoordinator = true;
			}
		}
        
        private void EntityCreateMany(List<Entity> list)
        {
            Game.Add(list);
        }

        private void EntityCreate(Entity entity)
        {
			Game.Add(entity);
        }

        private void EntityUpdate(Entity entity)
        {
            Game.Remove(entity);
            Game.Add(entity);
        }

        private void EntityDestroy(Entity entity)
        {
			Game.Remove(entity);
        }

	}
}
