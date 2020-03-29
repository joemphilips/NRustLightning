using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Primitives;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "release_ffi_channel_monitor",
            ExactSpelling = true)]
        static extern FFIResult _release_ffi_channel_monitor(IntPtr chan_mon);

        internal static FFIResult release_ffi_channel_monitor(IntPtr chan_mon, bool check = true)
        {
            return MaybeCheck(_release_ffi_channel_monitor(chan_mon), check);
        }
        
    }
}