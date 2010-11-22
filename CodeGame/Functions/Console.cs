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

namespace CodeGame.Functions
{
    class Console : FunctionTable
    {

        public static Drawables.LuaDrawable create(int w, int h, LuaFunction draw)
        {
            Drawables.LuaDrawable drawable = new Drawables.LuaDrawable(w, h, draw, null);
            Game.GetSingleton().AddDrawable(drawable);
            return drawable;
        }

        public static void setDraw(Drawables.LuaDrawable target, LuaFunction draw)
        {
            if (target == null) return;
            target.LDraw = draw;
        }

        public static void setKeyDown(Drawables.LuaDrawable target, LuaFunction keydown)
        {
            if (target == null) return;
            target.LKeyDown = keydown;
        }

        public static void toggleDraw(Drawables.LuaDrawable target)
        {
            if (target == null) return;
            target.isActive = !target.isActive;
        }

        public static void print(Drawables.LuaDrawable target, int x, int y, string str)
        {
            target.Console.print(x, y, str);
        }

        public string GetName()
        {
            return "console";
        }
    }
}
