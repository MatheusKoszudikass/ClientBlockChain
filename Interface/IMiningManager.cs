using ClientBlockChain.Entities.Enum;

namespace ClientBlockChain.Interface;

public interface IMiningManager
{
    Task Manager(ClientCommandMine commandMine);
}
