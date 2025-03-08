using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace ServerBlockChain.Interface
{
    public interface ISend
    {
        event Action<byte[]> SendingAtc;
        event Action<SslStream> ClientDisconnectedAtc;

        Task SendAsync(byte[] data);
    }
}