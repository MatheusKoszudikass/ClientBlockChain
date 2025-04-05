using System.Reflection;
using System.Runtime.InteropServices;
using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Client.Enum;

namespace ClientBlockChain.Util;

public static class InteropXmrig
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int XmrigStartDelegate(int argc, IntPtr argv);

    private static XmrigStartDelegate? _xmrigStart;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int XmrigStartUvDelegate();

    private static XmrigStartUvDelegate? _xmrigStartUv;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void XmrigStopUvDelegate();

    private static XmrigStopUvDelegate? _xmrigStopUv;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int XmrigIsStatusMiningDelegate();

    private static XmrigIsStatusMiningDelegate? _xmrigIsStatusMining;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int XmrigStopDelegate();

    private static XmrigStopDelegate? _xmrigStop;

    private static IntPtr _handle;

    private static bool Running { get; set; }

    public static int Start()
    {
        if (Running) return (int)ClientCommandXmrig.Running;

        _handle = LibraryExternManager.Load();

        if (_handle == IntPtr.Zero)
        {
            Console.WriteLine($"Error loading xmrig library: {_handle}");
            return 5;
        }

        _xmrigStart = Marshal.GetDelegateForFunctionPointer<XmrigStartDelegate>(
            LibraryExternManager.GetLibraryAddress("xmrig_start"));

        var args = new string[] { "" };
        var argvPtrs = new IntPtr[args.Length];

        for (var i = 0; i < args.Length; i++)
        {
            argvPtrs[i] = Marshal.StringToHGlobalAnsi(args[i]);
        }

        var argv = Marshal.AllocHGlobal(IntPtr.Size * argvPtrs.Length);
        if (argv == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failure by allocating memory to ARGV.");
        }

        Marshal.Copy(argvPtrs, 0, argv, argvPtrs.Length);

        try
        {
            Running = true;

            return _xmrigStart(args.Length, argv);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling xmrig start: {ex.Message}");
            return -1;
        }
        finally
        {
            foreach (var ptr in argvPtrs)
            {
                if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
            }

            if (argv != IntPtr.Zero) Marshal.FreeHGlobal(argv);
        }
    }

    public static int StartUv()
    {
        Console.WriteLine($"Thread do libuv: {Environment.CurrentManagedThreadId}");
        _xmrigStartUv = Marshal.GetDelegateForFunctionPointer<XmrigStartUvDelegate>(
            LibraryExternManager.GetLibraryAddress("xmrig_start_uv"));

        var result = _xmrigStartUv();

        Console.WriteLine($"Result operation StartUv: {result}");

        return result;
    }

    public static void StopUv()
    {
        _xmrigStopUv = Marshal.GetDelegateForFunctionPointer<XmrigStopUvDelegate>(
            LibraryExternManager.GetLibraryAddress("xmrig_stop_uv"));

        _xmrigStopUv!();

              LibraryExternManager.Free(_handle);

        Running = false;
        _handle = IntPtr.Zero;
    }

    public static int IsStatusMining()
    {
        if (_handle == IntPtr.Zero) return (int)ClientCommandXmrig.NotRunning;
        _xmrigIsStatusMining = Marshal.GetDelegateForFunctionPointer<XmrigIsStatusMiningDelegate>(
            LibraryExternManager.GetLibraryAddress("xmrig_status"));

        return _xmrigIsStatusMining!();
    }

    public static int Stop()
    {
        if (!Running) return (int)ClientCommandXmrig.NotRunning;

        _xmrigStop = Marshal.GetDelegateForFunctionPointer<XmrigStopDelegate>(
            LibraryExternManager.GetLibraryAddress("xmrig_stop"));

        var result = _xmrigStop();

        return result;
    }
}