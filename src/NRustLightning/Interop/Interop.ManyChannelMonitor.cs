using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;

namespace NRustLightning
{
    internal static partial class Interop
    {

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_many_channel_monitor",
            ExactSpelling = true)]
        private static extern FFIResult _create_many_channel_monitor(
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref FilterBlock filterBlock,
            ref ReEntered reEntered,
            
            ref BroadcastTransaction broadcastTransaction,
            ref Log log,
            ref GetEstSatPer1000Weight getEstSatPer1000Weight
            );

        internal static FFIResult create_many_channel_monitor(
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref FilterBlock filterBlock,
            ref ReEntered reEntered,
            
            ref BroadcastTransaction broadcastTransaction,
            ref Log log,
            ref GetEstSatPer1000Weight getEstSatPer1000Weight,
            
            bool check = true
            ) =>
            MaybeCheck(_create_many_channel_monitor(
                ref installWatchTx,
                ref installWatchOutPoint,
                ref watchAllTxn,
                ref getChainUtxo,
                ref filterBlock,
                ref reEntered,
                ref broadcastTransaction,
                ref log,
                ref getEstSatPer1000Weight
                ), check);

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "release_many_channel_monitor",
            ExactSpelling = true)]
        private static extern FFIResult _release_many_channel_monitor(IntPtr handle);

        internal static FFIResult release_many_channel_monitor(IntPtr handle, bool check = true) =>
            MaybeCheck(_release_many_channel_monitor(handle), check);
    }
}