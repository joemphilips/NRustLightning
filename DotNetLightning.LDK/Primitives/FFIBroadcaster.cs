using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Primitives
{
    internal ref struct FFIBroadcaster
    {
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void FFIBroadcastTransaction(UIntPtr tx_ptr, ulong tx_len);
}