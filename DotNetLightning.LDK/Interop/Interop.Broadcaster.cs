using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        [DllImport(RustLightning, EntryPoint = "create_broadcaster", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _create_broadcaster(ref FFIBroadcastTransaction fn, out BroadcasterHandle handle);

        internal static FFIResult create_broadcaster(
            ref FFIBroadcastTransaction fn,
            out BroadcasterHandle handle,
            bool check = true
            )
        {
            return MaybeCheck(_create_broadcaster(ref fn, out handle), check);
        }

        [DllImport(RustLightning, EntryPoint = "release_broadcaster", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _release_broadcaster(IntPtr handle);

        internal static FFIResult release_broadcaster(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_broadcaster(handle), check);
        }
        
        #if DEBUG
        [DllImport(RustLightning, EntryPoint = "ffi_test_broadcaster",
            ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _ffi_test_broadcaster(ref FFIBroadcaster broadcaster);

        internal static FFIResult ffi_test_broadcaster(ref FFIBroadcaster broadcaster, bool check = true)
        {
            return MaybeCheck(_ffi_test_broadcaster(ref broadcaster), check);
        }
        #endif
    }
}