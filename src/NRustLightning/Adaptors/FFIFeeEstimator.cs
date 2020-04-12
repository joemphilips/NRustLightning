using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong FFIGetEstSatPer1000Weight(ref FFITransaction tx);
}