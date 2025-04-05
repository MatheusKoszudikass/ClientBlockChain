using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Interface;
using ClientBlockChain.Util;

namespace ClientBlockChain.Handler;

public class ManagerTypeEventBus
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;

    public void PublishEventType(JsonElement data)
    {
        var obj = JsonElementConvert.ConvertToObject(data) ??
         throw new ArgumentNullException(nameof(data));

        switch (obj)
        {
            case ClientMine clientMine:
                _globalEventBus.Publish(clientMine);
                break;
            case Listener listener:
                _globalEventBus.Publish(listener);
                break;
            case LogEntry logEntry:
                _globalEventBus.Publish(logEntry);
                break;
            case ClientCommandMine clientCommandMine:
                _globalEventBus.Publish(clientCommandMine);
                break;
            case ClientCommandLog clientCommandLog:
                _globalEventBus.Publish(clientCommandLog);
                break;
            case HttpStatusCode httpStatusCode:
                _globalEventBus.Publish(httpStatusCode);
                break;
            case string message:
                _globalEventBus.Publish(message);
                break;
            default:
                throw new ArgumentException("Unsupported data type", nameof(data));
        }
    }

    public void PublishListEventType(List<JsonElement> listData)
    {
        var obj = new List<object>();

        foreach (var item in listData)
        {
            obj.Add(JsonElementConvert.ConvertToObject(item));
        }

        if (obj.All(o => o is ClientMine))
        {
            _globalEventBus.Publish(obj.Cast<ClientMine>().ToList());
        }
        else if(obj.All(o => o is LogEntry))
        {
            _globalEventBus.Publish(obj.Cast<LogEntry>().ToList());
        }
        else if(obj.All(o => o is ClientCommandMine))
        {
            _globalEventBus.Publish(obj.Cast<ClientCommandMine>().ToList());
        }
    }
}