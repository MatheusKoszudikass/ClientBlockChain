using System.Diagnostics;
using System.Runtime.InteropServices;
using Hardware.Info;


namespace ClientBlockchain.Entities;

public class HardwareInfomation
{
    private readonly IHardwareInfo _hardwareInfo = new HardwareInfo();
    public double Temperature { get; set; }
    public double CpuUsage { get; set; }
    public double GpuUsage { get; set; }
    public double RamUsage { get; set; }
    public double DiskUsage { get; set; }
    public int FanSpeed { get; set; }

    public string ProcessorName { get; private set; } = string.Empty;
    public string TotalRAM { get; private set; } = string.Empty;
    public string TotalDiskSpace { get; private set; } = string.Empty;

    // public HardwareInfomation()
    // {
    //     RefreshInfo();
    // }
    public void RefreshInfo()
    {
        _hardwareInfo.RefreshAll();

        var cpu = _hardwareInfo.CpuList.FirstOrDefault();
        ProcessorName = cpu?.Name ?? "Unknown";

        CpuUsage = GetCpuUsage();

        var memoryStatus = _hardwareInfo.MemoryStatus;
        TotalRAM = $"{memoryStatus.TotalPhysical / (1024 * 1024 * 1024):N2} GB";
        RamUsage = (double)(memoryStatus.TotalPhysical - memoryStatus.AvailablePhysical) / memoryStatus.TotalPhysical * 100;

        var disk = DriveInfo.GetDrives()
            .Where(d => d.DriveType == DriveType.Fixed);

        double totalSize = 0;
        double totalFreeSpace = 0;
        foreach (var drive in disk)
        {
            totalSize += drive.TotalSize;
            totalFreeSpace += drive.AvailableFreeSpace;
        }

        TotalDiskSpace = $"{totalSize / (1024 * 1024 * 1024):N2} GB";
        DiskUsage = ((totalSize - totalFreeSpace) / totalSize) * 100;
    }

    private double GetCpuUsage()
    {

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return GetCpuUsageWin();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return GetCpuUsageLinux();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return GetCpuUsageMac();

        return 0;
    }

    private static double GetCpuUsageWin()
    {
        try
        {
            // using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            // cpuCounter.NextValue();
            // Thread.Sleep(1000);
            // return cpuCounter.NextValue();
            return 0;

        }
        catch (Exception ex)
        {
           throw new Exception($"Error to get cpu use information: {ex.Message}");
        }
    }

    private static double GetCpuUsageLinux()
    {
        try
        {
            var cpuStats = File.ReadAllText("/proc/stat")
             .Split('\n')[0]
             .Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Skip(1)
             .Take(4)
             .Select(x => long.Parse(x))
             .ToArray();

            Thread.Sleep(1000);

            var cpuStats1 = File.ReadAllText("/proc/stat")
             .Split('\n')[0]
             .Split(' ', StringSplitOptions.RemoveEmptyEntries)
             .Skip(1)
             .Take(4)
             .Select(x => long.Parse(x))
             .ToArray();

            var idle1 = cpuStats[3];
            var idle2 = cpuStats1[3];
            var total1 = cpuStats.Sum();
            var total2 = cpuStats1.Sum();

            var idleDelta = idle2 - idle1;
            var totalDelta = total2 - total1;

            var cpuUsage = 100.0 * (1.0 - idleDelta / totalDelta);
            return cpuUsage;

        }
        catch
        {
            return 0;
        }
    }

    private static double GetCpuUsageMac()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "top",
                Arguments = "-l 2 -n 0",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd();
            if (output == null) return 0;

            var lines = output.Split('\n');
            var cpuLine = lines.FirstOrDefault(l => l.Contains("CPU usage:"));

            var cpuUsage = cpuLine?.Split(' ')
                .FirstOrDefault(s => s.EndsWith("%"))
                ?.TrimEnd('%');

            return double.TryParse(cpuUsage, out var result) ? result : 0;

        }
        catch
        {
            return 0;
        }
    }

    public override string ToString()
    {
        return $"""
                CPU: {ProcessorName}
                CPU Usage: {CpuUsage:N2}%
                RAM: {TotalRAM}
                RAM Usage: {RamUsage:N2}%
                Disk Space: {TotalDiskSpace}
                Disk Usage: {DiskUsage:N2}%
                """;
    }
}