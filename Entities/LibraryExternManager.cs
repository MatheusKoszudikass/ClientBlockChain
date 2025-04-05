using System.Runtime.InteropServices;
using ClientBlockChain.Entities.Library;

namespace ClientBlockChain.Entities;

public static class LibraryExternManager
{
    private static LibraryExternWin? _libraryExternWin;

    private static LibraryExternLinux? _libraryExternLinux;

    private static IntPtr _handle = IntPtr.Zero;

    public static IntPtr Load()
    {
        _handle = IntPtr.Zero;
        var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "libxmrig.dll"
            : "libxmrig.so";

        var libraryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "libs", fileName);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _libraryExternWin = new LibraryExternWin();
            _handle = _libraryExternWin.Load(libraryPath);

            CheckLibrary($"Error loading library: {_libraryExternWin.GetError()}");

            return _handle;
        }

        _libraryExternLinux = new LibraryExternLinux();
        _handle = _libraryExternLinux.Load(libraryPath);

        CheckLibrary($"Error loading library: {_libraryExternLinux.GetError()}");

        return _handle;
    }

    private static void CheckLibrary(string message)
    {
        if (_handle != IntPtr.Zero) return;

        throw new Exception(message);
    }

    public static bool Free(IntPtr handle)
    {
        if (handle == IntPtr.Zero) return false;

        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? _libraryExternWin!.Free(handle)
            : _libraryExternLinux!.Free(handle);
    }

    public static IntPtr GetLibraryAddress(string funcdllName)
    {
        CheckLibrary("Error not loading library.");
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? _libraryExternWin!.GetLibraryAddress(_handle, funcdllName)
            : _libraryExternLinux!.GetLibraryAddress(_handle, funcdllName);
    }
}