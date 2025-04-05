namespace ClientBlockChain.Entities.Abstract;

public abstract class LibraryExtern
{
    public abstract IntPtr Load(string path);
    public abstract bool Free(IntPtr handle);
    public abstract IntPtr GetLibraryAddress(IntPtr handle, string dllName);
    
    public abstract string GetError();
}