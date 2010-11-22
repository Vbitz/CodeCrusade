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
using Lidgren.Network;

namespace CodeGameShared
{
    public class Globals
    {
        public static NetPeerConfiguration config = new NetPeerConfiguration("CodeCrusade");
        public static NetPeerConfiguration configCli = new NetPeerConfiguration("CodeCrusade");
        public static int serverip = 13253;

        public static void Init()
        {
            config.MaximumConnections = 64;
            config.Port = 13253;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
        }
    }
}
