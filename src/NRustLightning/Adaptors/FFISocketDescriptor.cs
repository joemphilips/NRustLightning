using System;
using System.Runtime.InteropServices;
using Bool = System.Byte;

namespace NRustLightning.Adaptors
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate UIntPtr SendData(ref FFIBytes data, Bool resumeRead);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DisconnectSocket();
}