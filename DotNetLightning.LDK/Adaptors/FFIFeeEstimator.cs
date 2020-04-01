using System;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate ulong FFIGetEstSatPer1000Weight(ref FFITransaction tx);
    
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct FFIFeeEstimator
    {
        internal FFIGetEstSatPer1000Weight get_est_sat_per_1000_weight;
    }
}