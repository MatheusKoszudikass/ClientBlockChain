
using ClientBlockchain.Connection;
using ClientBlockchain.SystemOperation;
using ClientBlockChain.Entities;
using ClientBlockChain.InstructionsSocket;
using ClientBlockChain.Interface;
using ClientBlockChain.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Runtime.InteropServices;

class Program
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
            .AddSingleton<IClientMineService, ClientMineService>()
            .AddSingleton(typeof(IDataMonitorService<>), typeof(DataMonitorService<>))
            .AddSingleton<IDataConfirmationService, DataConfirmationService>()
            .AddSingleton<IStartClient, StartClient>()
            .BuildServiceProvider();

        var startClient = serviceProvide.GetRequiredService<IStartClient>();
        await startClient.Connect();

        Console.ReadLine();
    }
}