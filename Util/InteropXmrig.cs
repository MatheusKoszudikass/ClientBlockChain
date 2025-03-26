
using System.Runtime.InteropServices;
using ClientBlockChain.Entities.Client.Enum;

namespace ClientBlockChain.Util;

public static partial class InteropXmrig
{
    [LibraryImport("libxmrig", EntryPoint = "xmrig_start")]
    private static partial int XmrigStart(int argc, IntPtr argv);

    [LibraryImport("libxmrig", EntryPoint = "xmrig_start_uv")]
    private static partial int XmringStartUv();

    [LibraryImport("libxmrig", EntryPoint = "xmrig_status")]
    private static partial int IsMiningStatus();

    [LibraryImport("libxmrig", EntryPoint = "xmrig_stop")]
    private static partial int XmrigStop();

    public static async Task<int> Start()
    {
        string[] args = ["xmrig", "--donate-level=1", "--no-color"];

        IntPtr[] argvPtrs = new IntPtr[args.Length];

        for (int i = 0; i < args.Length; i++)
        {
            argvPtrs[i] = Marshal.StringToHGlobalAnsi(args[i]);
        }

        IntPtr argv = Marshal.AllocHGlobal(IntPtr.Size * argvPtrs.Length);
        Marshal.Copy(argvPtrs, 0, argv, argvPtrs.Length);

        Console.WriteLine("Chamando xmrig_start...");
        var result = XmrigStart(args.Length, argv);
        Console.WriteLine($"Resultado: {result}");

        foreach (IntPtr ptr in argvPtrs)
        {
            Marshal.FreeHGlobal(ptr);
        }

        Marshal.FreeHGlobal(argv);

        await Task.FromResult(true);

        return result;
    }

    public static int StartUv() => XmringStartUv();
    
    public static int IsStatusMining()
    {
        return IsMiningStatus();
    }
    public static int Stop() => XmrigStop();
}
