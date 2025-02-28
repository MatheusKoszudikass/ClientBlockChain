using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientBlockChain.Interface
{
    public interface IStartClient
    {
       Task Connect();
    }
}