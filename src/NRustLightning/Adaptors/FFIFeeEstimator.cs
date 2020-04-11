using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong FFIGetEstSatPer1000Weight(ref FFITransaction tx);
}