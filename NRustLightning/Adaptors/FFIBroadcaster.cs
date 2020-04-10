using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FFIBroadcastTransaction(ref FFITransaction tx);
    
}