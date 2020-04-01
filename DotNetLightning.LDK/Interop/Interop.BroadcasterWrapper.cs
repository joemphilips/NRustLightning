using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK
{
    #if DEBUG
    internal static partial class Interop
    {

        [DllImport(RustLightning, EntryPoint = "create_broadcaster_wrapper", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        internal static extern FFIResult _create_broadcaster_wrapper(ref FFIBroadcastTransaction fn, out BroadcasterWrapperHandle handle);

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
        internal static extern FFIResult _release_broadcaster_wrapper(IntPtr handle);

        internal static FFIResult release_broadcaster_wrapper(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_broadcaster_wrapper(handle), check);
        }
    }
    #endif
}