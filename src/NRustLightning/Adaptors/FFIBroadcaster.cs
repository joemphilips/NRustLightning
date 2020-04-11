using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FFIBroadcastTransaction(ref FFITransaction tx);
    
}