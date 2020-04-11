using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly ref struct FFIRoute
    {
        internal readonly IntPtr ptr;
        internal readonly UIntPtr len;

        public FFIRoute(IntPtr ptr, UIntPtr len)
        {
            this.ptr = ptr;
            this.len = len;
        }
    }
}

