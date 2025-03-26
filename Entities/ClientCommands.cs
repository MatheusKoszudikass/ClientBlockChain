using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientBlockChain.Entities.Enum;

namespace ClientBlockChain.Entities;

public class ClientCommands
{
    public ClientCommandMine ClientCommandMineCmd { get; set; }
    public ClientCommand ClientCommandCmd { get; set; }
}
