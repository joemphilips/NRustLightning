using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;
using NRustLightning.Handles;

namespace NRustLightning
{
    internal static partial class Interop
    {
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_ffi_channel_monitor",
            ExactSpelling = true)]
        static extern FFIResult _create_ffi_channel_monitor(
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref FilterBlock filterBlock,
            ref FFIBroadcastTransaction broadcastTransaction,
            ref Log log,
            ref FFIGetEstSatPer1000Weight getEstSatPer1000Weight,
            out ChannelMonitorHandle handle
            );

        internal static FFIResult create_ffi_channel_monitor(
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref FilterBlock filterBlock,
            ref FFIBroadcastTransaction broadcastTransaction,
            ref Log log,
            ref FFIGetEstSatPer1000Weight getEstSatPer1000Weight,
            out ChannelMonitorHandle handle,
            bool check = true
        )
            => MaybeCheck(_create_ffi_channel_monitor(ref installWatchTx, ref installWatchOutPoint, ref watchAllTxn, ref getChainUtxo, ref filterBlock, ref broadcastTransaction, ref log, ref getEstSatPer1000Weight, out handle), check);
        
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