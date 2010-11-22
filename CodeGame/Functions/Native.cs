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
using libtcod;

namespace CodeGame.Functions
{
    class Native : FunctionTable
    {
        public string GetName()
        {
            return "native";
        }

        public static void addChar(int x, int y, string nchar, int r, int g, int b)
        {
            Game game = Game.GetSingleton();
            game.GameSpace.setCharForeground(x, y, new TCODColor(r, g, b));
            game.GameSpace.setChar(x, y, nchar.ToCharArray()[0]);
        }

        public static void printColorString(int x, int y, string str, int r, int g, int b)
        {
            Game game = Game.GetSingleton();
            TCODColor currentforeground = game.GameSpace.getForegroundColor();
            game.GameSpace.setForegroundColor(new TCODColor(r, g, b));
            game.GameSpace.print(x, y, str);
            game.GameSpace.setForegroundColor(currentforeground);
        }

        public static void print(int x, int y, object str)
        {
            Game game = Game.GetSingleton();
            try
            {
                game.GameSpace.print(x, y, str.ToString());
            }
            catch (Exception ex)
            {
                game.WriteLine(ex.Message, TCODColor.red);
            }
        }

        public static void createFrame(int x, int y, int w, int h)
        {
            Game game = Game.GetSingleton();
            game.GameSpace.printFrame(x, y, w, h);
        }

        public static void setForeground(int r, int g, int b)
        {
            Game game = Game.GetSingleton();
            game.GameSpace.setForegroundColor(new TCODColor(r, g, b));
        }

        public static void fillScreen(int r, int b, int g)
        {
            Game game = Game.GetSingleton();
            game.GameSpace.setBackgroundColor(new TCODColor(r, b, g));
            game.GameSpace.clear();
        }

        public static int getFrameTime()
        {
            return TCODSystem.getFps();
        }

        public static void clear()
        {
            Game.GetSingleton().GameSpace.clear();
        }
    }
}
