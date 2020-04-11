using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;
using NRustLightning.Handles;

namespace NRustLightning
{
    #if DEBUG
    internal static partial class Interop
    {

        [DllImport(RustLightning, EntryPoint = "create_broadcaster_wrapper", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _create_broadcaster_wrapper(ref FFIBroadcastTransaction fn, out BroadcasterWrapperHandle handle);

        internal static FFIResult create_broadcaster_wrapper(
            ref FFIBroadcastTransaction fn,
            out BroadcasterWrapperHandle handle,
            bool check = true
            )
        {
            return MaybeCheck(_create_broadcaster_wrapper(ref fn, out handle), check);
        }
        [DllImport(RustLightning, EntryPoint = "release_broadcaster_wrapper", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _release_broadcaster_wrapper(IntPtr handle);

        internal static FFIResult release_broadcaster_wrapper(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_broadcaster_wrapper(handle), check);
        }

        [DllImport(RustLightning, EntryPoint = "test_broadcaster_wrapper", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern FFIResult _test_broadcaster_wrapper(BroadcasterWrapperHandle handle);

        internal static FFIResult test_broadcaster_wrapper(BroadcasterWrapperHandle handle, bool check = true)
        {
            return MaybeCheck(_test_broadcaster_wrapper(handle), check);
        }
    }
    #endif
}