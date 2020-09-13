using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Binding
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log(IntPtr this_arg, LDKLogger ldkLgger);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint GetEstSatPer1000Weight(IntPtr this_arg, LDKConfirmationTarget confirmationTarget);
    
    public class Class1
    {
        public unsafe void Run()
        {
        }
    }
}