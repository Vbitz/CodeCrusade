using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace CodeGameShared
{
    public class Game
    {
        public string Name;
        public List<Player> Players = new List<Player>();
        public Map CurrentMap = new Map();

        public Game(string name)
        {
            this.Name = name;
            CurrentMap.Generate();
        }
    }
}
