using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace ServerBlockChain.Interface
{
    public interface IReceive
    {
        event Action<byte[]>? ReceivedAtc;
        event Action<SslStream>?  ClientDesconnectedAct;

        Task ReceiveDataAsync();
    }
}