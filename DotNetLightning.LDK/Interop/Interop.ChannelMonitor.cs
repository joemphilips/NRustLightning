using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_ffi_channel_monitor",
            ExactSpelling = true)]
        static extern FFIResult _create_ffi_channel_monitor(
            ref FFIChainWatchInterface chainWatchInterface,
            ref FFIBroadcaster broadcaster,
            ref FFILogger logger,
            ref FFIFeeEstimator feeEstimator,
            out ChannelMonitorHandle handle
            );

        internal static FFIResult create_ffi_channel_monitor(
            ref FFIChainWatchInterface chainWatchInterface,
            ref FFIBroadcaster broadcaster,
            ref FFILogger logger,
            ref FFIFeeEstimator feeEstimator,
            out ChannelMonitorHandle handle,
            bool check = true
        )
            => MaybeCheck(_create_ffi_channel_monitor(ref chainWatchInterface, ref broadcaster, ref logger, ref feeEstimator, out handle), check);
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "release_ffi_channel_monitor",
            ExactSpelling = true)]
        static extern FFIResult _release_ffi_channel_monitor(IntPtr handle);
        
        internal static FFIResult release_ffi_channel_monitor(IntPtr handle, bool check = true)
        {
            return MaybeCheck(_release_ffi_channel_monitor(handle), check);
        }
        
    }
}