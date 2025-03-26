using ClientBlockChain.Entities;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service;

public class SendService<T> : ISend<T>
{

    public async Task SendAsync(T data,
        CancellationToken cts = default)
    {
        var send = new Send<T>(AuthenticateServer.SslStream!);

        if (data is List<T> listData)
        {
            await send!.SendListAsync(listData, cts);
        }
        await send.SendAsync(data, cts);
    }

    public async Task SendListAsync(List<T> listData,
        CancellationToken cts = default)
    {
        var sendList = new SendList<T>(AuthenticateServer.SslStream!);

        await sendList.SendListAsync(listData, cts);
    }
}
