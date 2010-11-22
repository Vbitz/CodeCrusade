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
    class Friend : FunctionTable
    {
        public static List<string> Friends = new List<string>();

        public static void add(string guid)
        {
            Friends.Add(guid);
        }

        public static void remove(string guid)
        {
            Friends.Remove(guid);
        }

        public static void removeAll()
        {
            Friends.Clear();
        }

        public static string getAll()
        {
            string retstr = "";
            foreach (string item in Friends)
            {
                retstr += item + ":";
            }
            retstr = retstr.Substring(0, retstr.Length - 1);
            return retstr;
        }

        public string GetName()
        {
            return "friend";
        }
    }
}
