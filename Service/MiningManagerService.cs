using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Client.Enum;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Handler;
using ClientBlockChain.Interface;
using ClientBlockChain.Util;
using System.Runtime.InteropServices;

namespace ClientBlockChain.Service;

public class MiningManagerService : IMiningManager
{
    private readonly IIlogger<LogEntry> _ilogger;
    private readonly GlobalEventBus _globalEventBus;
    private readonly IDataMonitor<ClientCommandXmrig> _dataMonitor;
    private Thread? _uvThread;
    private CancellationTokenSource _cts = new();

    public MiningManagerService(
        IIlogger<LogEntry> ilogger,
        GlobalEventBus globalEventBus,
        IDataMonitor<ClientCommandXmrig> dataMonitor)
    {
        _ilogger = ilogger;
        _globalEventBus = globalEventBus;
        _dataMonitor = dataMonitor;
        globalEventBus.Subscribe<ClientCommandMine>(OnClientCommand);
    }

    public async Task Manager(ClientCommandMine commandMine)
    {
        try
        {
            Console.WriteLine(commandMine.ToString());
            switch (commandMine)
            {
                case ClientCommandMine.Start:
                    StartMining();
                    break;
                case ClientCommandMine.Status:
                    IsStatusMining();
                    break;
                case ClientCommandMine.Stop:
                    StopMining();
                    break;
                default:
                    await _ilogger.Log(commandMine, "Unknown command", LogLevel.Error);
                    break;
            }
        }
        catch (DllNotFoundException ex)
        {
            await _ilogger.Log(commandMine, ex, $"DLL not found (StartMining): {ex.Message}", LogLevel.Error);
        }
        catch (EntryPointNotFoundException ex)
        {
            await _ilogger.Log(commandMine, ex, $"SO/DLL entry point not found (StartMining): {ex.Message}", LogLevel.Error);
        }
        catch (MarshalDirectiveException ex)
        {
            await _ilogger.Log(commandMine, ex, $"Marshal directive exception (StartMining): {ex.Message}", LogLevel.Error);
        }
        catch (OutOfMemoryException ex)
        {
            await _ilogger.Log(commandMine, ex, $"Memory not sufficient (StartMining): {ex.Message}", LogLevel.Error);
        }
        catch (IOException ex)
        {
            await _ilogger.Log(commandMine, ex, $"Error stopping mining (StopMining): {ex.Message}", LogLevel.Error);
            _globalEventBus.Publish(Listener.Instance);
        }
        catch (Exception ex)
        {
            await _ilogger.Log(commandMine, ex, $"Unexpected error (StartMining): {ex.Message}", LogLevel.Critical);
        }
    }

    private void StartMining()
    {
        var result = InteropXmrig.Start();
        SendResultOperationXmrig(result);

        if (result != 0)
            return;

        Console.WriteLine($"Thread principal: {Thread.CurrentThread.ManagedThreadId}");

        _uvThread = new Thread(() => InteropXmrig.StartUv());

        _uvThread.Start();
    }

    private void IsStatusMining()
    {
        var result = InteropXmrig.IsStatusMining();
        Console.WriteLine(result);
        SendResultOperationXmrig(result);
    }

    private void StopMining()
    {
        Console.WriteLine($"Thread principal entrada StopMining: {Environment.CurrentManagedThreadId}");

        try
        {
            // Cancela operações que dependem do token (caso usado futuramente)
            _cts.Cancel();

            var result = InteropXmrig.Stop();
            // Solicita ao loop libuv que pare
            InteropXmrig.StopUv();

            // Espera thread libuv encerrar
            // _uvThread?.Join();
            // _uvThread = null;

            Console.WriteLine($"Thread de saida do libuv: {Environment.CurrentManagedThreadId}");

            // Agora é seguro parar o minerador
            SendResultOperationXmrig(result);

            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro durante StopMining: {ex.Message}");
        }
        finally
        {
            // _cts.Dispose();
            // _cts = new CancellationTokenSource();
        }

        Console.WriteLine($"Thread principal saida StopMining: {Environment.CurrentManagedThreadId}");
    }

    private void SendResultOperationXmrig(int result)
    {
        switch (result)
        {
            case -1:
                _dataMonitor.SendDataAsync(ClientCommandXmrig.Stop);
                break;
            case 0:
                _dataMonitor.SendDataAsync(ClientCommandXmrig.Start);
                break;
            case 1:
                _dataMonitor.SendDataAsync(ClientCommandXmrig.Running);
                break;
            case -2:
                _dataMonitor.SendDataAsync(ClientCommandXmrig.NotRunning);
                break;
            case 2:
                _dataMonitor.SendDataAsync(ClientCommandXmrig.NoValidConfig);
                break;
            default:
                _dataMonitor.SendDataAsync(ClientCommandXmrig.Erro);
                break;
        }
    }

    private void OnClientCommand(ClientCommandMine data)
    {
        _ = Task.Run(() => Manager(data));
    }
}
