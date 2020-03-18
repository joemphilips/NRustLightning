using System.Runtime.InteropServices;
using DotNetLightning.LDK.Primitives;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {

        [DllImport(RustLightning, CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void* create_ffi_channel_manager(
            byte* seed_ptr,
            Network* n,
            in NullFFILogger logger_ptr,
            ref UserConfig config);
    }
}