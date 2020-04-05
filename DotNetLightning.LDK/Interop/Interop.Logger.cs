using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        [DllImport(RustLightning, EntryPoint = "create_logger", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _create_logger(ref Log fn, out LoggerHandle handle);

        internal static FFIResult create_logger(
            ref Log fn,
            out LoggerHandle handle,
            bool check = true
            )
        {
            return MaybeCheck(_create_logger(ref fn, out handle), check);
        }
        
        [DllImport(RustLightning, EntryPoint = "release_logger", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _release_logger(IntPtr handle);

        internal static FFIResult release_logger(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_broadcaster(handle), check);
        }

#if DEBUG
        [DllImport(RustLightning, EntryPoint = "test_logger", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _test_logger(LoggerHandle handle);

        internal static FFIResult test_logger(LoggerHandle handle, bool check = true)
            => MaybeCheck(_test_logger(handle), check);
#endif

    }
}