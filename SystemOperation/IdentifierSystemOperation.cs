using System.Runtime.InteropServices;

namespace ClientBlockChain.SystemOperation;

public class IdentifierSystemOperation
{
    public static OSPlatform GetOS()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return OSPlatform.Windows;

        return OSPlatform.Linux;
    }

}
