using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ClientBlockChain.Entity;

namespace ClientBlockChain.Interface
{
    public interface IClientMineService
    {
        Task ClientMineInfoAsync(Listener listener);
    }
}