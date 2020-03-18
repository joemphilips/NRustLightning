using System.Reflection;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Primitives
{
    internal delegate void NullLog(NullFFILogger self);
    [StructLayout(LayoutKind.Sequential)]
    internal struct NullFFILogger
    {
        private NullLog log_ptr;
    }
}