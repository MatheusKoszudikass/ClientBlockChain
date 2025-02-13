using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientBlockChain.Entity
{
    public class SocketConnectedEventHandler(Socket socket) : EventArgs
    {
        public Socket SocketConnected = socket;
    }
}