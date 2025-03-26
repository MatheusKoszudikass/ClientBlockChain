using System.Runtime.InteropServices;
using ClientBlockChain.Entities;
using ClientBlockChain.Handler;
using ClientBlockChain.Interface;
using ClientBlockChain.Service;
using Microsoft.Extensions.DependencyInjection;

namespace ClientBlockChain;

internal static partial class Program
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GetConsoleWindow();

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);
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
            .AddSingleton<IClientMine, ClientMineService>()
            .AddSingleton(typeof(IDataMonitor<>), typeof(DataMonitorService<>))
            .AddSingleton(typeof(IEventApp<>), typeof(EventAppService<>))
            .AddSingleton<ILoggerSend, LoggerSendService>()
            .AddSingleton<IManagerFunctionality, ManagerFunctionalityService>()
            .AddSingleton<IMiningManager, MiningManagerService>()
            .AddSingleton(typeof(IIlogger<>), typeof(LoggerService<>))
            .AddSingleton<IReceive, ReceiveService>()
            .AddSingleton(typeof(ISend<>), typeof(SendService<>))
            .AddTransient<IStartClient, StartClient>()
            .BuildServiceProvider();

        _ = serviceProvide.GetRequiredService<ILoggerSend>();
        _ = serviceProvide.GetRequiredService<IMiningManager>();
        var startClient = serviceProvide.GetRequiredService<IStartClient>();
        await startClient.Connect();
        _ = serviceProvide.GetRequiredService<IReceive>().ReceiveDataAsync();
        Console.ReadLine();
    }
}