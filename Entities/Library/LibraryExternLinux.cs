using System.Runtime.InteropServices;
using ClientBlockChain.Entities.Abstract;

namespace ClientBlockChain.Entities.Library;

public partial class LibraryExternLinux : LibraryExtern
{
    private const string LibraryName = "libdl.so.2";
    
    private const int RtldLazy = 1;
    private const int RtldNow = 2;
    private const int RtldGlobal = 8;
    private const int RtldLocal = 4;
    
    [LibraryImport(LibraryName, EntryPoint = "dlopen", 
        SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr LoadDlopenLinux(string path, int flags);
    
    [LibraryImport(LibraryName, EntryPoint = "dlsym",
        SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr GetProcAddressLinux(IntPtr handle, string symbol);
    
    [LibraryImport(LibraryName, EntryPoint = "dlclose", 
        SetLastError = true)]
    private static partial int FreeLibraryLinux(IntPtr handle);
    
    [LibraryImport(LibraryName, EntryPoint = "dlerror",
        SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr GetErrorLinux();
    
    public override IntPtr Load(string path)
    {
        var handler = LoadDlopenLinux(path, RtldLazy | RtldGlobal);
        
        return handler;
    }
    
    public override IntPtr GetLibraryAddress(IntPtr handle, 
        string dllName) => GetProcAddressLinux(handle, dllName);
    
    public override bool Free(IntPtr handle) => 
        handle != IntPtr.Zero && FreeLibraryLinux(handle) == 0;
    
    public override string GetError() => 
        Marshal.PtrToStringAnsi(GetErrorLinux()) ?? "Erro null";
}