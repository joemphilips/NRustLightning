using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    public enum FFIConfirmationTarget : int
    {
        Background = 0,
        Normal,
        HighPriority
    }
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong GetEstSatPer1000Weight(FFIConfirmationTarget tx);
}