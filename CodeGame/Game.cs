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
using System.Reflection;
using System.Threading;
using System.IO;
using LuaInterface;
using CodeGameShared;
using Lidgren.Network;
using libtcod;

namespace CodeGame
{
    class Game
    {
        static Game Singleton;
        public Lua lua = new Lua();
        public NetClient client;
        Thread netThread = new Thread(new ParameterizedThreadStart(NetLoop));
        public List<IDrawable> Drawables = new List<IDrawable>();
        private List<IDrawable> DrawableCue = new List<IDrawable>();
        public Drawables.Console TConsole = new Drawables.Console();
        public TCODConsole GameSpace = new TCODConsole(80, 60);
        public static string playerName = "NOTHING";
        public static string CurrentGame = "NOTHING";
        public Map CurrentMap;
        public int PlayerResources = 0;
        Drawables.GameMap GameMap = new Drawables.GameMap();

        public Game()
        {
            Globals.Init();
            client = new NetClient(Globals.configCli);
            client.Start();
        }

        public void AddDrawable(IDrawable drawable)
        {
            DrawableCue.Add(drawable);
        }

        public static void NetLoop(object args)
        {
            try
            {
                Game game = (Game)args;
                while (true)
                {
                    while (game.client.MessageAvailable)
                    {
                        NetIncomingMessage msg = game.client.ReadMessage();
                        switch (msg.MessageType)
                        {
                            case NetIncomingMessageType.VerboseDebugMessage:
                            case NetIncomingMessageType.WarningMessage:
                            case NetIncomingMessageType.DebugMessage:
                            case NetIncomingMessageType.ErrorMessage:
                                game.TConsole.WriteLine(msg.ReadString());
                                break;
                            case NetIncomingMessageType.Data:
                                string nick = msg.ReadString();
                                string str = msg.ReadString();
                                if (str.StartsWith("L:"))
                                {
                                    try
                                    {
                                        if (Functions.Friend.Friends.Contains(str.Split(':')[1]))
                                        {
                                            game.lua.DoString(str.Split(':')[2]);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        break;
                                    }
                                }
                                else if (nick == "PRIV")
                                {
                                    string nMSG = msg.ReadString();
                                    if (str.Contains(playerName))
                                    {
                                        game.TConsole.WriteLine(str + ":" + nMSG, TCODColor.yellow);
                                    }
                                    else
                                    {
                                        game.TConsole.WriteLine(str + ":" + nMSG, TCODColor.yellow);
                                    }
                                }
                                else if (nick == "GAMEJOIN")
                                {
                                    CurrentGame = str;
                                    game.TConsole.WriteLine("Getting Map");
                                    game.CurrentMap = new Map();
                                    game.CurrentMap.Load(msg.ReadBytes(2500));
                                    game.GameMap.active = true;
                                    game.PlayerResources = 10000;
                                }
                                else if (nick == "GAMERES")
                                {
                                    game.PlayerResources = int.Parse(str);
                                }
                                else
                                {
                                    if (nick == "###FILE###")
                                    {
                                        int num = msg.ReadInt16();
                                        byte[] bytes = msg.ReadBytes(num);
                                        game.TConsole.WriteLine("Reciving Msg");
                                        WriteBinaryFile("client_file.bin", bytes);
                                    }
                                    if (str.Contains(playerName))
                                    {
                                        game.TConsole.WriteLine(nick + ":" + str, TCODColor.purple);
                                    }
                                    else
                                    {
                                        game.TConsole.WriteLine(nick + ":" + str, TCODColor.orange);
                                    }
                                }
                                break;
                            case NetIncomingMessageType.ConnectionApproval:
                                game.TConsole.WriteLine("Connection Aproved", TCODColor.green);
                                break;
                            case NetIncomingMessageType.StatusChanged:
                                game.TConsole.WriteLine("Server Status Changed To: " + msg.SenderConnection.Status.ToString(), TCODColor.green);
                                break;
                            default:
                                game.TConsole.WriteLine("Can Not Pause Message Type: " + msg.MessageType.ToString(), TCODColor.red);
                                break;
                        }
                    }
                    Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }

        private static void WriteBinaryFile(string filename, byte[] item)
        {
            BinaryWriter writer = new BinaryWriter(System.IO.File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write));
            writer.Write(item, 2, item.Length - 2);
            writer.Close();
        }

        public void Run()
        {
            RegisterFunctions(Assembly.GetExecutingAssembly(), "CodeGame.Functions");
            netThread.Start(this);
            TCODConsole.setCustomFont("arial10x10.png", (int)TCODFontFlags.LayoutTCOD | (int)TCODFontFlags.Grayscale);
            TCODConsole.initRoot(80, 60, "CodeCrusade");
            TCODSystem.setFps(60);
            //Thread.Sleep(10);
            if (File.Exists("client_init.lua"))
            {
                lua.DoFile("client_init.lua");
            }
            MainLoop();
            netThread.Abort();
            client.Shutdown("Client Closing");
        }

        private void MainLoop()
        {
            while (!TCODConsole.isWindowClosed())
            {
                TCODConsole.flush();
                Draw();
                Logic();
                Thread.Sleep(0);
            }
        }

        private static void Logic()
        {
            float lastTime = TCODSystem.getLastFrameLength();
            Functions.Timer.Update(lastTime);
        }

        private void Draw()
        {
            TCODKey key = TCODConsole.checkForKeypress();
            TCODConsole.blit(GameSpace, 0, 0, 80, 60, TCODConsole.root, 0, 0);
            if (Drawables.Count > 0)
            {
                foreach (IDrawable item in Drawables)
                {
                    if (item.IsActive())
                    {
                        item.Draw(TCODConsole.root);
                        item.KeyPress(key);
                    }
                }
            }
            if (DrawableCue.Count > 0)
            {
                foreach (IDrawable item in DrawableCue)
                {
                    Drawables.Add(item);
                }
                DrawableCue.Clear();
            }
            if (GameMap.active)
            {
                GameMap.Draw(TCODConsole.root);
            }
            TConsole.Draw(TCODConsole.root);
            TConsole.KeyPress(key);
        }

        private void RegisterFunctions(Assembly program, string space)
        {
            foreach (Module item in program.GetModules())
            {
                foreach (Type item2 in item.GetTypes())
                {
                    if (item2.Namespace == space && item2.GetInterface("CodeGame.FunctionTable") != null)
                    {
                        object instance = Activator.CreateInstance(item2);
                        string table = (string)item2.GetMethod("GetName").Invoke(instance, null);
                        if (table != "")
                        {
                            lua.NewTable(table);
                        }
                        foreach (MethodBase item3 in item2.GetMethods())
                        {
                            if (item3.IsStatic == true)
                            {
                                if (table != "")
                                {
                                    lua.RegisterFunction(table + "." + item3.Name, item2, item3);
                                }
                                else
                                {
                                    lua.RegisterFunction(item3.Name, item2, item3);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void WriteLine(object line)
        {
            TConsole.WriteLine(line);
        }

        public void WriteLine(object line, TCODColor col)
        {
            TConsole.WriteLine(line, col);
        }

        public static Game GetSingleton()
        {
            if (Singleton == null)
            {
                Singleton = new Game();
            }
            return Singleton;
        }
    }
}
