using System;
using System.Runtime.InteropServices;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK
{
    internal static partial class Interop
    {
            
        private static FFIResult MaybeCheck(FFIResult result, bool check)
        {
            return check ? result.Check() : result;
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_channel_manager",
            ExactSpelling = true)]
        private static unsafe extern FFIResult _create_ffi_channel_manager(
            byte* seed_ptr,
            UIntPtr seed_len,
            in Network n,
            in UserConfig config,
            ChannelMonitorHandle monitor,
            LoggerHandle logger_ptr,
            BroadcasterHandle broadcaster,
            FeeEstimatorHandle fee_est,
            ulong current_block_height,
            out ChannelManagerHandle handle
            );

        internal static unsafe FFIResult create_ffi_channel_manager(
            byte* seed_ptr,
            UIntPtr seed_len,
            in Network n,
            in UserConfig config,
            ChannelMonitorHandle monitor,
            LoggerHandle logger_ptr,
            BroadcasterHandle broadcaster,
            FeeEstimatorHandle fee_est,
            ulong current_block_height,
            out ChannelManagerHandle handle,
            bool check = true
        )
        {
            return MaybeCheck(_create_ffi_channel_manager(seed_ptr, seed_len, in n , in config, monitor, logger_ptr, broadcaster, fee_est, current_block_height, out handle), check);
        }

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