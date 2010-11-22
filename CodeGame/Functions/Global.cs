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

namespace CodeGame.Functions
{
    class Global : FunctionTable
    {
        public string GetName()
        {
            return "";
        }

        public static void print(object item)
        {
            Game game = Game.GetSingilton();
            try
            {
                game.TConsole.WriteLine(item, libtcod.TCODColor.white);
            }
            catch (Exception ex)
            {
                game.TConsole.WriteLine(ex.Message, libtcod.TCODColor.red);
            }
        }

        public static void printc(object item, int r, int g, int b)
        {
            Game.GetSingilton().WriteLine(item, new libtcod.TCODColor(r, g, b));
        }

        public static void say(object item)
        {
            Game game = Game.GetSingilton();
            try
            {
                Networking.say(item.ToString());
            }
            catch (Exception ex)
            {
                game.TConsole.WriteLine(ex.Message, libtcod.TCODColor.red);
            }
        }

        public static void exit()
        {
            Game.GetSingilton().client.Disconnect("Client Exit");
            Environment.Exit(0);
        }

        public static void history()
        {
            Game game = Game.GetSingilton();
            foreach (string item in game.TConsole.lastLines)
            {
                game.TConsole.WriteLine(item);
            }
        }
    }
}
