using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace CodeGameShared
{
    public class Game
    {
        public string Name;
        public List<Player> Players = new List<Player>();
        public Map CurrentMap = new Map();

        public Game(string name)
        {
            this.Name = name;
            this.CurrentMap.Generate();
        }

        public void UpdateAllResources(NetServer server)
        {
            foreach (Player item in this.Players)
            {
                this.UpdateResource(server, item);
            }
        }

        public void UpdateResource(NetServer server, Player ply)
        {
            try
            {
                NetOutgoingMessage msg = server.CreateMessage();
                msg.Write("GAMERES");
                msg.Write(ply.Resources.ToString());
                ply.Connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
            }
            catch (Exception) { }
        }
    }
}
