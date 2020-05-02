using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;
using NRustLightning.Handles;

namespace NRustLightning
{
    internal static partial class Interop
    {
        [DllImport(RustLightning, EntryPoint = "create_broadcaster", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _create_broadcaster(ref BroadcastTransaction fn, out BroadcasterHandle handle);

        internal static FFIResult create_broadcaster(
            ref BroadcastTransaction fn,
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
        internal static extern FFIResult _ffi_test_broadcaster(BroadcasterHandle handle);

        internal static FFIResult ffi_test_broadcaster(BroadcasterHandle handle, bool check = true)
        {
            return MaybeCheck(_ffi_test_broadcaster(handle), check);
        }
        #endif
    }
}