using System.Reflection;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Primitives
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void NullLog(NullFFILogger self);
    
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct NullFFILogger
    {
        private NullLog log_ptr;
    }
}