using System.Runtime.InteropServices;
using ClientBlockChain.Entities.Abstract;
using Hardware.Info;

namespace ClientBlockChain.Entities.HadwareInfo;

public class CpuInfo : HardwareInfoBase
{

    public CpuInfo()
    {
        Status = "Active";
        GetCpuInfo();
    }

    public void GetCpuInfo()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            GetNameCpu();
            GetCpuUsageWin();
            GetCoreCpu();
            return;
        }
        GetNameCpu();
        GetCpuUsageLinux();
        GetCoreCpu();
    }
    
    public void GetNameCpu()
    {
        var hardwareInfo = new HardwareInfo();

        hardwareInfo.RefreshCPUList();
        var name = hardwareInfo.CpuList.FirstOrDefault();
        Name = name?.Name ?? "Unknown";
    }

    public void GetCoreCpu()
    {
        var hardwareInfo = new HardwareInfo();

        hardwareInfo.RefreshCPUList();
        var cpu = hardwareInfo.CpuList.FirstOrDefault();
        TotalCapacity = cpu?.NumberOfCores ?? 0;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetSystemTimes(out long lpIdleTime, out long lpKernelTime, out long lpUserTime);
    private long _prevIdleTime, _prevKernelTime, _prevUserTime;

    public void GetCpuUsageWin()
    {
        if (!GetSystemTimes(out long idleTime, out long kernelTime, out long userTime))
            UsagePercentage = 0;

        long idleDiff = idleTime - _prevIdleTime;
        long totalDiff = (kernelTime - _prevKernelTime) + (userTime - _prevUserTime);

        _prevIdleTime = idleTime;
        _prevKernelTime = kernelTime;
        _prevUserTime = userTime;

        UsagePercentage = totalDiff == 0 ? 0 : 100.0 * (1.0 - (double)idleDiff / totalDiff);
    }

    public void GetCpuUsageLinux()
    {
        var cpuStats1 = ReadCpuStatsLinux();
        var idle1 = cpuStats1[3];
        var total1 = cpuStats1.Sum();

        Thread.Sleep(500);

        var cpuStats2 = ReadCpuStatsLinux();
        var idle2 = cpuStats2[3];
        var total2 = cpuStats2.Sum();

        var idleDelta = idle2 - idle1;
        var totalDelta = total2 - total1;

        UsagePercentage = totalDelta == 0 ? 0 : 100.0 * (1.0 - (double)idleDelta / totalDelta);
    }

    private static long[] ReadCpuStatsLinux()
    {
        return File.ReadAllText("/proc/stat")
            .Split('\n')[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(long.Parse)
            .ToArray();
    }
}
