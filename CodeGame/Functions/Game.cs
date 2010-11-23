using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using CodeGameShared;

namespace CodeGame.Functions
{
    class CGame : FunctionTable
    {
        public static void join(string gameName)
        {
            Game game = Game.GetSingleton();
            try
            {
                NetOutgoingMessage strMsg = game.client.CreateMessage();
                strMsg.Write("JOIN " + gameName);
                game.client.SendMessage(strMsg, NetDeliveryMethod.ReliableOrdered);
            }
            catch (Exception ex)
            {
                Utility.WriteLogLine(ex.Message, "engine");
                throw;
            }
        }

        public string GetName()
        {
            return "game";
        }
    }
}
