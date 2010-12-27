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
using System.Threading;

namespace CodeGameServer
{
    class Program
    {
        static List<Player> Connections = new List<Player>();
        static NetServer server;
        static List<Game> Games = new List<Game>();

        static void Main(string[] args)
        {
            Globals.Init();
            server = new NetServer(Globals.config);
            WriteLine("Building Map");
            WriteLine("Server Starting");
            server.Start();
            while (true)
            {
                while (server.MessageAvailable)
                {
                    NetIncomingMessage msg = server.ReadMessage();
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.Data:
                            string netStr = msg.ReadString();
                            switch (netStr.Substring(0, 4))
                            {
                                case "NICK":
                                    string newNick = netStr.Substring(4);
                                    string str1 = " : " + GetPlayerNick(msg.SenderConnection) + " : Changed there Nickname to " + newNick;
                                    WriteLine(newNick + str1);
                                    if (Connections.Count > 0)
                                    {
                                        for (int i = 0; i < Connections.Count; i++)
                                        {
                                            if (Connections[i].Connection == msg.SenderConnection)
                                            {
                                                Player update = Connections[i];
                                                update.Name = newNick;
                                                Connections[i] = update;
                                            }
                                        }
                                    }
                                    SendToAll("[Server]", str1);
                                    break;
                                case "SAY ":
                                    string nick = GetPlayerNick(msg.SenderConnection);
                                    string str = netStr.Substring(4);
                                    WriteLine(nick + " : " + str);
                                    SendToAll(nick, str);
                                    break;
                                case "PRIV":
                                    netStr = netStr.Substring(5);
                                    string nickname = netStr.Substring(0, netStr.IndexOf(' '));
                                    NetOutgoingMessage msg3 = server.CreateMessage();
                                    NetConnection conn = GetPlayerConn(nickname);
                                    if (conn != null)
                                    {
                                        msg3.Write("PRIV");
                                        msg3.Write(GetPlayerNick(msg.SenderConnection));
                                        msg3.Write(netStr.Substring(nickname.Length));
                                        server.SendMessage(msg3, conn, NetDeliveryMethod.ReliableOrdered, 0);
                                    }
                                    break;
                                case "JOIN":
                                    string game = netStr.Substring(5);
                                    Player gamePlayer = GetPlayer(msg.SenderConnection);
                                    string pnick = gamePlayer.Name;
                                    WriteLine(pnick + " Joining : " + game);
                                    NetOutgoingMessage msg4 = server.CreateMessage();
                                    msg4.Write("GAMEJOIN");
                                    Game playerGame = GetGame(game);
                                    if (playerGame == null)
                                    {
                                        playerGame = new Game(game);
                                        Games.Add(playerGame);
                                    }
                                    msg4.Write(game);
                                    playerGame.Players.Add(gamePlayer);
                                    playerGame.CurrentMap.GenerateMapPacket(msg4);
                                    gamePlayer.Resources = 10000;
                                    server.SendMessage(msg4, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                                    break;
                                default:
                                    NetOutgoingMessage msg2 = server.CreateMessage();
                                    msg2.Write("[Server] : Server Command not Found, You Wrote : " + netStr);
                                    msg.SenderConnection.SendMessage(msg2, NetDeliveryMethod.ReliableOrdered);
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            ReadStatus(msg);
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            WriteLine("New Client Connected from : " + msg.SenderEndpoint.ToString());
                            msg.SenderConnection.Approve();
                            SendToAll("[Server]", "New Client Connected");
                            Connections.Add(new Player("Player", msg.SenderConnection));
                            //SendFile("server_file.bin", msg.SenderConnection);
                            break;
                        default:
                            WriteLine("Can Not Pause Message Type: " + msg.MessageType.ToString());
                            break;
                    }
                }
                Thread.Sleep(10);
            }
        }

        public static Game GetGame(string name)
        {
            foreach (Game item in Games)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        private static void ReadStatus(NetIncomingMessage msg)
        {
            switch (msg.SenderConnection.Status)
            {
                case NetConnectionStatus.Connected:
                    break;
                case NetConnectionStatus.Connecting:
                    break;
                case NetConnectionStatus.Disconnected:
                    try
                    {
                        Player ply = GetPlayer(msg.SenderConnection);
                        if (Connections.Contains(ply))
                        {
                            Connections.Remove(ply);
                        }
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    break;
                case NetConnectionStatus.Disconnecting:
                    break;
                case NetConnectionStatus.None:
                    break;
                default:
                    break;
            }
        }

        private static void SendFile(string FileName, NetConnection sendto)
        {
            byte[] item = System.IO.File.ReadAllBytes(FileName);
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write("###FILE###");
            msg.Write("");
            msg.Write(item.Length + 2);
            msg.Write(item);
            sendto.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        public static void SendToAll(string nickname, string message)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write(nickname);
            msg.Write(message);
            server.SendMessage(msg, GetConnections(), NetDeliveryMethod.ReliableOrdered, 0);
        }

        public static void WriteLine(object line)
        {
            Console.WriteLine(line);
            Utility.WriteLogLine(line, "server");
        }

        private static string GetPlayerNick(NetConnection connection)
        {
            foreach (Player item in Connections)
            {
                if (item.Connection == connection)
                {
                    return item.Name;
                }
            }
            return "Player";
        }

        private static Player GetPlayer(NetConnection connection)
        {
            foreach (Player item in Connections)
            {
                if (item.Connection == connection)
                {
                    return item;
                }
            }
            return null;
        }

        private static NetConnection GetPlayerConn(string name)
        {
            foreach (Player item in Connections)
            {
                if (item.Name == name)
                {
                    return item.Connection;
                }
            }
            return null;
        }

        private static List<NetConnection> GetConnections()
        {
            List<NetConnection> conns = new List<NetConnection>();
            foreach (Player item in Connections)
            {
                conns.Add(item.Connection);
            }
            return conns;
        }
    }
}
