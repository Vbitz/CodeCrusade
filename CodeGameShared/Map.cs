using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.IO;

namespace CodeGameShared
{

    public class Map
    {
        public byte[,] Items = new byte[50, 50];
        
        public void Generate()
        {
            Random rand = new Random();
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    int num = rand.Next(1, 20);
                    if (num < 10)
                    {
                        Items[x, y] = 0;
                    }
                    else if (num < 15)
                    {
                        Items[x, y] = 1;
                    }
                    else if (num < 17)
                    {
                        Items[x, y] = 2;
                    }
                    else
                    {
                        Items[x, y] = 3;
                    }
                }
            }
        }

        public void Load(byte[] data)
        {
            if (data.Length < 2500)
            {
                throw new ArgumentException("Length of stream incorrect");
            }
            int counter = 0;
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    Items[x, y] = data[counter];
                    counter++;
                }
            }
        }

        public NetOutgoingMessage GenerateMapPacket(NetOutgoingMessage template)
        {
            byte[] map = new byte[2500];
            int counter = 0;
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    map[counter] = Items[x, y];
                    counter++;
                }
            }
            template.Write(map);
            return template;
        }
    }
}
