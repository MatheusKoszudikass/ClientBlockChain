using ClientBlockChain.Interface;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Handler;
using ClientBlockChain.Entities;

namespace ClientBlockChain.Service;

public class ManagerFunctionalityService : IManagerFunctionality
{
    private readonly IMiningManager _miningManager;
    private readonly IIlogger<ClientCommand> _ilogger;
    private ClientCommands? _clientCommands;

    public ManagerFunctionalityService(IMiningManager miningManager,
     IIlogger<ClientCommand> Ilogger, GlobalEventBus globalEventBus)
    {
        _miningManager = miningManager;
        _ilogger = Ilogger;
        globalEventBus = GlobalEventBus.InstanceValue;
        globalEventBus.Subscribe<ClientCommands>(OnCommandServer);
    }

    public void Manager()
    {
        switch(_clientCommands!.ClientCommandCmd)
        {
            case ClientCommand.ClientMining: 
                _ilogger.Log(ClientCommand.ClientMining, "Start mining", LogLevel.Information);
                break;
            default :
                _ilogger.Log(ClientCommand.ClientMining, "Command not found", LogLevel.Error);
                break;
        }
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public void OnCommandServer(ClientCommands data)
    {
        _clientCommands = data;
        Manager();
    }
}
