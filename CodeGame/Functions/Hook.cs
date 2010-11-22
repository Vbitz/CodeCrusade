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
    class Hook : FunctionTable
    {
        static Dictionary<string, Dictionary<string, LuaFunction>> table = new Dictionary<string, Dictionary<string, LuaFunction>>();

        public static void add(string name, string uni, LuaFunction funct)
        {
            if (!table.ContainsKey(name))
            {
                table.Add(name, new Dictionary<string, LuaFunction>());
            }
            table[name].Add(uni, funct);
        }

        public static void call(string name)
        {
            if (table.ContainsKey(name))
            {
                foreach (LuaFunction item in table[name].Values)
                {
                    item.Call();
                }
            }
        }

        public static void remove(string uni)
        {
            foreach (KeyValuePair<string, Dictionary<string, LuaFunction>> item in table)
            {
                if (item.Value.ContainsKey(uni))
                {
                    item.Value.Remove(uni);
                }
            }
        }

        public string GetName()
        {
            return "hook";
        }
    }
}
