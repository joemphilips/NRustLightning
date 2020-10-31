using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Binding
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void BroadcastTx(IntPtr this_arg, LDKTransaction tx);
    
}