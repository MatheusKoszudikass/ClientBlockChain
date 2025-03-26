
namespace ClientBlockChain.Entities.HadwareInfo
{
    public static class DiskInfo
    {
        public static double GetDiskUsage()
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);
            var totalSpace = drives.Sum(d => d.TotalSize);
            var usedSpace = totalSpace - drives.Sum(d => d.AvailableFreeSpace);
            Console.WriteLine(totalSpace == 0 ? 0 : (double)100 * usedSpace / totalSpace);
            return totalSpace == 0 ? 0 : (double)100 * usedSpace / totalSpace;
        }

        public static string GetTotalDiskSpace()
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed);
            var totalSpace = drives.Sum(d => d.TotalSize);
            return $"{totalSpace / (1024 * 1024 * 1024):N2} GB";
        }
    }
}