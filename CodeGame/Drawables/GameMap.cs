using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;

namespace CodeGame.Drawables
{
    class GameMap : IDrawable
    {
        TCODConsole Console = new TCODConsole(50, 50);
        bool isDrawed = false;
        public bool active = false;

        public void Draw(TCODConsole target)
        {
            Game game = Game.GetSingleton();
            if (!isDrawed && game.CurrentMap != null)
            {

                Console.clear();
                for (int x = 0; x < 50; x++)
                {
                    for (int y = 0; y < 50; y++)
                    {
                        int num = (int)game.CurrentMap.Items[x, y];
                        switch (num)
                        {
                            case 0:
                                Console.setCharForeground(x, y, TCODColor.green);
                                Console.setChar(x, y, '.');
                                break;
                            case 1:
                                Console.setCharForeground(x, y, TCODColor.grey);
                                Console.setChar(x, y, '+');
                                break;
                            case 2:
                                Console.setCharForeground(x, y, TCODColor.darkGreen);
                                Console.setChar(x, y, '^');
                                break;
                            case 3:
                                Console.setCharForeground(x, y, TCODColor.blue);
                                Console.setChar(x, y, '*');
                                break;
                            default:
                                break;
                        }
                    }
                }
                isDrawed = true;
            }
            TCODConsole.blit(Console, 0, 0, 50, 50, target, 2, 2);
        }

        public void KeyPress(TCODKey key)
        {

        }

        public bool IsActive()
        {
            return active;
        }
    }
}
