using System.Runtime.InteropServices;
using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;
using ClientBlockChain.Interface;
using ClientBlockchain.Service;
using ClientBlockChain.Service;
using Microsoft.Extensions.DependencyInjection;
using ClientBlockchain.Entities;

namespace ClientBlockchain;

static class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private const int SW_HIDE = 0;
    static async Task Main(string[] args)
    {

        // if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        // {
        //     IntPtr handle = GetConsoleWindow();
        //     if(handle != IntPtr.Zero) ShowWindow(handle, SW_HIDE);
        // }

        var serviceProvide = new ServiceCollection()
            .AddSingleton(GlobalEventBus.InstanceValue)
            .AddSingleton(AuthenticateServer.Instance)
            .AddSingleton<IClientMineService, ClientMineService>()
            .AddSingleton(typeof(IDataMonitorService<>), typeof(DataMonitorService<>))
            .AddSingleton<IDataConfirmationService, DataConfirmationService>()
            .AddSingleton(typeof(IEventApp<>), typeof(EventAppService<>))
            .AddSingleton(typeof(IJsonManager<>), typeof(JsonManager<>))
            .AddSingleton<ILoggerSend, LoggerSendService>()
            .AddSingleton(typeof(IIlogger<>), typeof(LoggerService<>))
            .AddSingleton(typeof(IReceive<>), typeof(ReceiveService<>))
            .AddSingleton(typeof(ISend<>), typeof(SendService<>))
            .AddTransient<IStartClient, StartClient>()
            .BuildServiceProvider();

         _ = serviceProvide.GetRequiredService<ILoggerSend>();
        var startClient = serviceProvide.GetRequiredService<IStartClient>();
        await startClient.Connect();

        Console.ReadLine();
    }
}