using System.Text;
using System.Text.Json;
using ServerBlockChain.Handler;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service;

public class JsonManager<T> : IJsonManager<T> where T : class
{
    private readonly GlobalEventBus _globalEventBus;
    private readonly ManagerTypeEventBus _managerTypeEventBus;

    public JsonManager(GlobalEventBus globalEventBus)
    {
        _globalEventBus = globalEventBus;
        _managerTypeEventBus = new ManagerTypeEventBus();
        // _globalEventBus.Subscribe<T>(message => Serialize(message));
        // _globalEventBus.Subscribe<T>(messageByte => Serialize(messageByte));
        // _globalEventBus.Subscribe<string>(message => Desserialize(message));
        _globalEventBus.Subscribe<byte[]>(messageByte => Desserialize(messageByte));
    }
    public string Serialize(T obj)
    {
        var result = JsonSerializer.Serialize(obj);
        _managerTypeEventBus.PublishEventType(obj!);
        return result;
    }

    public byte[] SerializeByte(T obj)
    {
        var result = JsonSerializer.SerializeToUtf8Bytes(obj);
        return result;
    }
    
    public T Desserialize(string json)
    {
        var result = JsonSerializer.Deserialize<T>(json) ?? null!;
        _managerTypeEventBus.PublishEventType(result!);
        return result;
    }

    public T Desserialize(byte[] data)
    {
        var obj = Encoding.UTF8.GetString(data);
        var result = JsonSerializer.Deserialize<T>(obj) ?? null!;
        _managerTypeEventBus.PublishEventType(result!);
        return result;
    }
}