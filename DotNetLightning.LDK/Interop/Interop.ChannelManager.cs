using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Primitives;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
            
        private static FFIResult MaybeCheck(FFIResult result, bool check)
        {
            return check ? result.Check() : result;
        }

        [DllImport(RustLightning, CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void* create_ffi_channel_manager(
            byte* seed_ptr,
            int seed_len,
            in Network n,
            in UserConfig config,
            in FFIManyChannelMonitor monitor,
            in NullFFILogger logger_ptr,
            in FFIBroadcaster broadcaster,
            in FFIFeeEstimator fee_est,
            ulong current_block_height,
            out ChannelManagerHandle handle
            );

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "release_ffi_channel_manager",
            ExactSpelling = true)]
        static unsafe extern FFIResult _release_ffi_channel_manager(IntPtr chan_man);

        internal static FFIResult release_ffi_channel_manager(
            IntPtr chan_man,
            bool check = true
        ) => MaybeCheck(_release_ffi_channel_manager(chan_man), check);
    }
}