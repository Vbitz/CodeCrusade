//    CodeCursade\CodeGame: Coding Game
//    Copyright (C) 2010 Vbitz
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGameShared;
using Lidgren.Network;
using System.Net;

namespace CodeGame.Functions
{
    class Networking : FunctionTable
    {
        public string GetName()
        {
            return "net";
        }

        public static void connect(string ip)
        {
            Game game = Game.GetSingleton();
            try
            {
                game.client.Connect(ip, Globals.serverip);
            }
            catch (Exception ex)
            {
                Utility.WriteLogLine(ex.Message, "engine");
            }
        }

        public static void say(string msg)
        {
            Game game = Game.GetSingleton();
            try
            {
                NetOutgoingMessage strMsg = game.client.CreateMessage();
                strMsg.Write("SAY " + msg);
                game.client.SendMessage(strMsg, NetDeliveryMethod.ReliableOrdered);
            }
            catch (Exception ex)
            {
                Utility.WriteLogLine(ex.Message, "engine");
                throw;
            }
        }

        public static void nick(string newNick)
        {
            Game game = Game.GetSingleton();
            try
            {
                NetOutgoingMessage strMsg = game.client.CreateMessage();
                strMsg.Write("NICK" + newNick);
                game.client.SendMessage(strMsg, NetDeliveryMethod.ReliableOrdered);
                Game.playerName = newNick;
            }
            catch (Exception ex)
            {
                Utility.WriteLogLine(ex.Message, "engine");
                throw;
            }
        }

        public static string sendLua(object chunk, string key)
        {
            try
            {
                return "L:" + key + ":" + chunk.ToString();
            }
            catch (Exception) { }
            return "";
        }

        public static void privMsg(string nick, string msg)
        {
            Game game = Game.GetSingleton();
            try
            {
                NetOutgoingMessage strMsg = game.client.CreateMessage();
                strMsg.Write("PRIV " + nick + " " + msg);
                game.client.SendMessage(strMsg, NetDeliveryMethod.ReliableOrdered);
            }
            catch (Exception ex)
            {
                Utility.WriteLogLine(ex.Message, "engine");
                throw;
            }
        }

        public static string getPastebin(string id)
        {
            WebClient client = new WebClient();
            string str = client.DownloadString("http://pastebin.com/raw.php?i=" + id);
            int startindex = str.IndexOf("<pre>") + 5;
            int endindex = str.IndexOf("</pre>");
            str = str.Substring(startindex, endindex - startindex);
            return str;
        }
    }
}
