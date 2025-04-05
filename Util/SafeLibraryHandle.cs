using System.Runtime.InteropServices;
using ClientBlockChain.Entities.Library;

namespace ClientBlockChain.Util;

public class GenericSafeHandle : SafeHandle
{
    private readonly Func<IntPtr, bool> _releaseFunction;

    public GenericSafeHandle(IntPtr handle, Func<IntPtr, bool> releaseFunction)
        : base(IntPtr.Zero, ownsHandle: true)
    {
        SetHandle(handle);
        _releaseFunction = releaseFunction;
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        return _releaseFunction(handle);
    }
}