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
        internal static extern FFIResult _create_logger(ref Log fn, out LoggerHandle handle);

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
        internal static extern FFIResult _release_logger(IntPtr handle);

        internal static FFIResult release_logger(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_broadcaster(handle), check);
        }
        
    }
}