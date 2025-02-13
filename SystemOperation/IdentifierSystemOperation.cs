using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClientBlockchain.SystemOperation
{
    public class IdentifierSystemOperation
    {
        public static OSPlatform GetOS()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return OSPlatform.Windows;

            return OSPlatform.Linux;
        }

    }
}