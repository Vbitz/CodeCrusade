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
    struct STimer
    {
        public float currentTime;
        public int endTime;
        public LuaFunction funct;

        static public STimer CreateTimer(int end, LuaFunction funct)
        {
            STimer newTimer = new STimer();
            newTimer.currentTime = 0;
            newTimer.endTime = end;
            newTimer.funct = funct;
            return newTimer;
        }

    }

    class Timer : FunctionTable
    {
        public string GetName()
        {
            return "timer";
        }

        public static List<STimer> timers = new List<STimer>();

        public static void create(int time, LuaFunction funct)
        {
            timers.Add(STimer.CreateTimer(time, funct));
        }

        public static void Update(float lastTime)
        {
            if (timers.Count > 0)
            {
                for (int x = 0; x < timers.Count; x++)
                {
                    STimer current = timers[x];
                    current.currentTime += lastTime;
                    if (current.currentTime > current.endTime)
                    {
                        current.funct.Call();
                        timers.RemoveAt(x);
                    }
                    else
                    {
                        timers[x] = current;
                    }
                }
            }
        }
    }
}
