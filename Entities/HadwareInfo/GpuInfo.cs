
using System.Runtime.InteropServices;
using ClientBlockChain.Util;
using ClientBlockChain.Entities.Abstract;
using Hardware.Info;
using LibreHardwareMonitor.Hardware;

namespace ClientBlockChain.Entities.HadwareInfo;

public class GpuInfo : HardwareInfoBase
{
    public string Manufacturer { get; private set; } = "Empty Manufacturer";
    public string VRAM { get; private set; } = "Empty VRAM";
    public string VRAMUsed { get; private set; } = "Empty VRAM Used";
    public string DriverVersion { get; private set; } = "Empty Driver Version";
    public string Resolution { get; private set; } = "Empty Resolution";
    public string RefreshRate { get; private set; } = "Empty Refresh Rate";

    public GpuInfo()
    {
        Status = "Active";
        GetGpu();
    }

    private void GetGpu()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            GetGpuWin();
            // GetSystemUsage();
            return;
        }
        GetSystemUsage();
        GetGpuLinux();
    }

#pragma warning disable CA1416 // Validate platform compatibility
    private void GetGpuWin()
    {
        var hard = new HardwareInfo();
        hard.RefreshVideoControllerList();
        foreach (var gpu in hard.VideoControllerList)
        {
            Name = gpu.Name;
            Manufacturer = gpu.Manufacturer;
            VRAM = $"{Convert.ToUInt64(gpu.AdapterRAM) / (1024 * 1024)} GB";
            DriverVersion = gpu.DriverVersion;
            Resolution = $"{gpu.CurrentHorizontalResolution} x {gpu.CurrentVerticalResolution}";
            RefreshRate = gpu.CurrentRefreshRate.ToString();
            Console.WriteLine(gpu.Name);
        }
    }

    private static void GetSystemUsage()
    {
        var computer = new Computer()
        {
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsCpuEnabled = true,
            IsControllerEnabled = true
        };

        computer.Open();

        GetCpuUsage(computer);
        GetGpuUsage(computer);
        GetMemoryUsage(computer);
        GetStorageUsage(computer);

        computer.Close();
    }

    private static void GetCpuUsage(Computer computer)
    {
        foreach (IHardware hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.Cpu)
            {
                Console.WriteLine($"CPU: {hardware.Name}");
                hardware.Update();
                ProcessSensors(hardware, "CPU");
            }
        }
    }

    private static void GetGpuUsage(Computer computer)
    {
        foreach (IHardware hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.GpuNvidia
                || hardware.HardwareType == HardwareType.GpuAmd
                || hardware.HardwareType == HardwareType.GpuIntel)
            {
                Console.WriteLine($"GPU: {hardware.Name}");
                hardware.Update();
                ProcessSensors(hardware, "GPU");
            }
        }
    }

    private static void GetMemoryUsage(Computer computer)
    {
        foreach (IHardware hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.Memory)
            {
                Console.WriteLine($"Memory: {hardware.Name}");
                hardware.Update();
                ProcessSensors(hardware, "Memory");
            }
        }
    }

    private static void GetStorageUsage(Computer computer)
    {
        foreach (IHardware hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.Storage)
            {
                Console.WriteLine($"Storage: {hardware.Name}");
                hardware.Update();
                ProcessSensors(hardware, "Disk");
            }
        }
    }

    private static void ProcessSensors(IHardware hardware, string componentType)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            switch (sensor.SensorType)
            {
                case SensorType.Load:
                    Console.WriteLine($"{componentType} Usage: {sensor.Value}%");
                    break;

                case SensorType.Temperature:
                    Console.WriteLine($"{componentType} Temperature: {sensor.Value}°C");
                    break;

                case SensorType.Data:
                    string unit = componentType == "GPU" ? "MB" : "GB";
                    string dataType = componentType == "GPU" ? "VRAM Used" : "Used";
                    Console.WriteLine($"{componentType} {dataType}: {sensor.Value} {unit}");
                    break;
            }
        }
    }

#pragma warning restore CA1416 // Validate platform compatibility

    private void GetGpuLinux()
    {
        Name = CommandTerminal.ExecuteBashCommandLinux("lspci | grep -i 'vga\\|3d\\|display' | awk -F ': ' '{print $2}'");
        Manufacturer = CommandTerminal.ExecuteBashCommandLinux("lspci -vnn | grep VGA -A 12 "
        + "| grep 'Subsystem' | awk -F ': ' '{print $2}'");
        // GetGpuUsageLinux();
        Vram();
        Resolution = CommandTerminal.ExecuteBashCommandLinux("xrandr | grep '*' | awk '{print $1}'");
        RefreshRate = CommandTerminal.ExecuteBashCommandLinux("xrandr | grep '*' | awk '{print $2}'");
    }

    private void Vram()
    {
        VRAM = CommandTerminal.ExecuteBashCommandLinux("glxinfo | grep 'Video memory' | awk '{print $3}'");
        if (string.IsNullOrWhiteSpace(VRAM))
        {
            VRAM = CommandTerminal.ExecuteBashCommandLinux("nvidia-smi --query-gpu=memory.total --format=csv,noheader,nounits");
        }
    }

    private void GetGpuUsageLinux()
    {
        var gpuUsageCommand = "if command -v nvidia-smi &> /dev/null; then " +
                      "nvidia-smi --query-gpu=memory.used,memory.total,utilization.gpu --format=csv,noheader,nounits; " +
                      "elif [ -f '/sys/class/drm/card0/device/mem_info_vram_used' ]; then " +
                      "used=$(cat /sys/class/drm/card0/device/mem_info_vram_used); " +
                      "total=$(cat /sys/class/drm/card0/device/mem_info_vram_total); " +
                      "echo \"$(expr $used / 1024 / 1024) MB, $(expr $total / 1024 / 1024) MB, Desconhecido\"; " +
                      "else echo 'Desconhecido, Desconhecido, Desconhecido'; fi";

        var gpuUsage = CommandTerminal.ExecuteBashCommandLinux(gpuUsageCommand).Split(',');
        VRAMUsed = gpuUsage[0].Trim() + " MB";
        TotalCapacity = Convert.ToDouble(gpuUsage[1].Trim()) / 1024 / 1024;
    }
}
