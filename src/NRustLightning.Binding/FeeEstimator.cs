using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Binding
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint GetEstSatPer1000Weight(IntPtr this_arg, LDKConfirmationTarget confirmationTarget);
    
}