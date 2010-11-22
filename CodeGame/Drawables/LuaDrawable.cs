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
using LuaInterface;
using libtcod;

namespace CodeGame.Drawables
{
    class LuaDrawable : IDrawable
    {
        public int x = 0;
        public int y = 0;
        public bool isActive = false;
        public LuaFunction LDraw;
        public LuaFunction LKeyDown;
        public TCODConsole Console;
        private List<LuaDrawable> Parents = new List<LuaDrawable>();

        public LuaDrawable(int w, int h, LuaFunction ldraw, LuaFunction lkeydown)
        {
            Console = new TCODConsole(w, h);
            LDraw = ldraw;
            LKeyDown = lkeydown;
        }

        public void Draw(libtcod.TCODConsole target)
        {
            Console.clear();
            if (LDraw != null)
            {
                try
                {
                    LDraw.Call();
                }
                catch (Exception ex)
                {
                    Game.GetSingleton().TConsole.WriteLine(ex.Message, TCODColor.red);
                }
            }
            TCODConsole.blit(Console, 0, 0, Console.getWidth(), Console.getHeight(), target, x, y);
        }

        public void KeyPress(libtcod.TCODKey key)
        {
            if (LKeyDown != null)
            {
                LKeyDown.Call();
            }
        }

        public bool IsActive()
        {
            return isActive;
        }
    }
}
