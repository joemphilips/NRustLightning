using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FFIBroadcastTransaction(ref FFITransaction tx);
    
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct FFIBroadcaster
    {
        internal FFIBroadcastTransaction broadcast_transaction;
    }
}