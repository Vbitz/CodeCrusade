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
using libtcod;

namespace CodeGame.Drawables
{
    class ConsoleLine
    {
        public string Str;
        public TCODColor Color;

        public ConsoleLine(string str)
        {
            Str = str;
            Color = TCODColor.grey;
        }

        public ConsoleLine(string str, TCODColor col)
        {
            Str = str;
            Color = col;
        }
    }

    class Console : IDrawable
    {
        private bool consoleActive = false;
        public List<ConsoleLine> consoleLines = new List<ConsoleLine>();
        private TCODConsole TConsole = new TCODConsole(78, 28);
        private string ConsoleInput = "";
        private int Position = 0;
        public List<string> lastLines = new List<string>();
        public int CurrentLine = 0;
        private string TotalInput = "";

        public Console()
        {

        }

        public void Draw(TCODConsole target)
        {
            if (consoleActive)
            {
                TConsole.clear();
                TConsole.setForegroundColor(TCODColor.white);
                TConsole.printFrame(0, 0, 78, 28);
                TConsole.print(2, 0, "Console");
                if (ConsoleInput.Length > 0)
                {
                    if (ConsoleInput.Length < 76)
                    {
                        string stemp = ConsoleInput.Substring(0, Position);
                        if (Position < ConsoleInput.Length)
                        {
                            string stemp2 = ConsoleInput.Substring(Position, ConsoleInput.Length - Position);
                            TConsole.print(stemp.Length + 2, 25, stemp2);
                        }
                        TConsole.print(1, 25, stemp);
                        TConsole.setForegroundColor(TCODColor.red);
                        TConsole.print(stemp.Length + 1, 25, "+");
                    }
                }
                else
                {
                    TConsole.setForegroundColor(TCODColor.red);
                    TConsole.print(1, 25, "+");
                }
                int y = 0;
                foreach (ConsoleLine item in consoleLines)
                {
                    TConsole.setForegroundColor(item.Color);
                    TConsole.print(1, y++ + 1, item.Str);
                }
                TCODConsole.blit(TConsole, 0, 0, 78, 28, target, 1, 1);
            }
        }

        public void KeyPress(TCODKey key)
        {
            Game game = Game.GetSingilton();
            if (key.KeyCode == TCODKeyCode.Char)
            {
                if (key.Character != '`')
                {
                    if (consoleActive == true)
                    {
                        AddToInput(key.Character);
                    }
                }
                else
                {
                    consoleActive = !consoleActive;
                    game.lua["native.console"] = consoleActive;
                }
            }
            else if (key.KeyCode == TCODKeyCode.Space)
            {
                if (consoleActive == true)
                {
                    AddToInput(' ');
                }
            }
            else if (key.KeyCode == TCODKeyCode.Backspace)
            {
                if (consoleActive == true)
                {
                    if (ConsoleInput.Length > 0)
                    {
                        string stemp = ConsoleInput.Substring(0, Position - 1);
                        if (Position < ConsoleInput.Length)
                        {
                            ConsoleInput = stemp + ConsoleInput.Substring(Position, ConsoleInput.Length - Position);
                        }
                        else
                        {
                            ConsoleInput = stemp;
                        }
                        Position--;
                    }
                }
            }
            else if (key.KeyCode == TCODKeyCode.Delete)
            {
                if (consoleActive == true)
                {
                    if (ConsoleInput.Length > 0 && Position < ConsoleInput.Length)
                    {
                        ConsoleInput = ConsoleInput.Remove(Position, 1);
                    }
                }
            }
            else if (key.KeyCode == TCODKeyCode.Enter & key.Shift == false)
            {
                if (consoleActive == true)
                {
                    try
                    {
                        if (TotalInput == "")
                        {
                            WriteLine('>' + ConsoleInput, TCODColor.lightGrey);
                            game.lua.DoString(ConsoleInput);
                        }
                        else
                        {
                            TotalInput += ConsoleInput + Environment.NewLine;
                            WriteLine("<->" + ConsoleInput, TCODColor.lightestPurple);
                            game.lua.DoString(TotalInput);
                            TotalInput = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine(ex.Message, TCODColor.red);
                    }
                    if (lastLines.Count > 0)
                    {
                        if (lastLines[lastLines.Count - 1] != ConsoleInput)
                        {
                            lastLines.Add(ConsoleInput);
                        }
                    }
                    else
                    {
                        lastLines.Add(ConsoleInput);
                    }
                    CurrentLine = lastLines.Count;
                    ConsoleInput = "";
                    Position = 0;
                }
            }
            else if (key.KeyCode == TCODKeyCode.Down)
            {
                if (CurrentLine < lastLines.Count - 1)
                {
                    CurrentLine++;
                    ConsoleInput = lastLines[CurrentLine];
                    Position = ConsoleInput.Length;
                }
            }
            else if (key.KeyCode == TCODKeyCode.Up)
            {
                if (CurrentLine > 0)
                {
                    CurrentLine--;
                    ConsoleInput = lastLines[CurrentLine];
                    Position = ConsoleInput.Length;
                }
            }
            else if (key.KeyCode == TCODKeyCode.Left)
            {
                if (Position > 0)
                {
                    Position--;
                }
            }
            else if (key.KeyCode == TCODKeyCode.Right)
            {
                if (Position < ConsoleInput.Length)
                {
                    Position++;
                }
            }
            else if (key.KeyCode == TCODKeyCode.Home)
            {
                Position = 0;
            }
            else if (key.KeyCode == TCODKeyCode.End)
            {
                Position = ConsoleInput.Length;
            }
            else
            {
                if (consoleActive)
                {
                    if (key.Shift)
                    {
                        switch (key.KeyCode)
                        {
                            case TCODKeyCode.One:
                                AddToInput('!');
                                break;
                            case TCODKeyCode.Two:
                                AddToInput('@');
                                break;
                            case TCODKeyCode.Three:
                                AddToInput('#');
                                break;
                            case TCODKeyCode.Four:
                                AddToInput('$');
                                break;
                            case TCODKeyCode.Six:
                                AddToInput('^');
                                break;
                            case TCODKeyCode.Seven:
                                AddToInput('&');
                                break;
                            case TCODKeyCode.Eight:
                                AddToInput('*');
                                break;
                            case TCODKeyCode.Nine:
                                AddToInput('(');
                                break;
                            case TCODKeyCode.Zero:
                                AddToInput(')');
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (key.KeyCode)
                        {
                            case TCODKeyCode.One:
                                AddToInput('1');
                                break;
                            case TCODKeyCode.Two:
                                AddToInput('2');
                                break;
                            case TCODKeyCode.Three:
                                AddToInput('3');
                                break;
                            case TCODKeyCode.Four:
                                AddToInput('4');
                                break;
                            case TCODKeyCode.Five:
                                AddToInput('5');
                                break;
                            case TCODKeyCode.Six:
                                AddToInput('6');
                                break;
                            case TCODKeyCode.Seven:
                                AddToInput('7');
                                break;
                            case TCODKeyCode.Eight:
                                AddToInput('8');
                                break;
                            case TCODKeyCode.Nine:
                                AddToInput('9');
                                break;
                            case TCODKeyCode.Zero:
                                AddToInput('0');
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (key.Shift && key.KeyCode == TCODKeyCode.Enter)
            {
                if (consoleActive && ConsoleInput.Length > 0)
                {
                    TotalInput += ConsoleInput + Environment.NewLine;
                    WriteLine("<->" + ConsoleInput, TCODColor.lightestPurple);
                    ConsoleInput = "";
                    Position = 0;
                }
            }
        }

        public void WriteLine(object item)
        {
            WriteLine(item, TCODColor.grey);
        }

        public void WriteLine(object item, TCODColor col)
        {
            string item2 = item.ToString();
            AddLine(item2, col);
        }

        private void AddLine(string item, TCODColor col)
        {
            consoleLines.Add(new ConsoleLine(item, col));
            if (consoleLines.Count > 24)
            {
                consoleLines.RemoveAt(0);
            }
        }

        public bool IsActive()
        {
            return true;
        }

        private void AddToInput(char input)
        {
            if (Position < ConsoleInput.Length)
            {
                ConsoleInput = ConsoleInput.Insert(Position, input.ToString());
            }
            else
            {
                ConsoleInput += input;
            }
            Position++;
        }
    }
}
