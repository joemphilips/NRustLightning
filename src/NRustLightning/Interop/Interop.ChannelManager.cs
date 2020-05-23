using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using NRustLightning.Adaptors;
using NRustLightning.Handles;

namespace NRustLightning
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
            Network* n,
            UserConfig* config,
            
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref BroadcastTransaction broadcastTransaction,
            ref Log log,
            ref GetEstSatPer1000Weight getEstSatPer1000Weight,
            ulong current_block_height,
            out ChannelManagerHandle handle
            );

        internal static unsafe FFIResult create_ffi_channel_manager(
            byte* seed_ptr,
            UIntPtr seed_len,
            Network* n,
            UserConfig* config,
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref BroadcastTransaction broadcastTransaction,
            ref Log log,
            ref GetEstSatPer1000Weight getEstSatPer1000Weight,
            ulong current_block_height,
            out ChannelManagerHandle handle,
            bool check = true
        )
        {
            return MaybeCheck(_create_ffi_channel_manager(seed_ptr, seed_len, n , config, ref installWatchTx, ref installWatchOutPoint, ref watchAllTxn, ref getChainUtxo, ref broadcastTransaction, ref log, ref getEstSatPer1000Weight, current_block_height, out handle), check);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "list_channels",
            ExactSpelling = true)]
        static extern FFIResult _list_channels(IntPtr bufOut, UIntPtr bufLen, out UIntPtr actualChannelsLen, ChannelManagerHandle handle);
        internal static FFIResult list_channels(IntPtr bufOut, UIntPtr bufLen, out UIntPtr actualChannelsLen, ChannelManagerHandle handle)
        {
            return MaybeCheck(_list_channels(bufOut, bufLen, out actualChannelsLen, handle), true);
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_channel",
            ExactSpelling = true)]
        static extern FFIResult _create_channel(FFIPublicKey publicKey, ulong channelValueSatoshis, ulong pushMsat, ulong userId, ChannelManagerHandle handle);
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_channel_with_custom_config",
            ExactSpelling = true)]
        static extern FFIResult _create_channel_with_custom_config(FFIPublicKey publicKey, ulong channelValueSatoshis, ulong pushMsat, ulong userId, in UserConfig config, ChannelManagerHandle handle);

        internal static FFIResult create_channel(FFIPublicKey publicKey, ulong channelValueSatoshis, ulong pushMsat,
            ulong userId, ChannelManagerHandle handle, in UserConfig? config = null)
        {
            if (config is null)
            {
                return MaybeCheck(_create_channel(publicKey, channelValueSatoshis, pushMsat, userId, handle), true);
            }
            var v = config.Value;
            return MaybeCheck(_create_channel_with_custom_config(publicKey, channelValueSatoshis, pushMsat, userId, in v, handle), true);
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "close_channel",
            ExactSpelling = true)]
        static extern unsafe FFIResult _close_channel(byte* channelId, ChannelManagerHandle handle);
        internal static unsafe FFIResult close_channel(byte* channelId, ChannelManagerHandle handle)
        {
            return MaybeCheck(_close_channel(channelId, handle), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "foce_close_channel",
            ExactSpelling = true)]
        static extern FFIResult _force_close_channel(ref Array32 channelId, ChannelManagerHandle handle);
        internal static FFIResult force_close_channel(ref Array32 channelId, ChannelManagerHandle handle)
        {
            return MaybeCheck(_force_close_channel(ref channelId, handle), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "foce_close_all_channels",
            ExactSpelling = true)]
        static extern FFIResult _force_close_all_channels(ChannelManagerHandle handle);
        internal static FFIResult force_close_all_channels(ChannelManagerHandle handle)
        {
            return MaybeCheck(_force_close_all_channels(handle), true);
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "send_payment",
            ExactSpelling = true)]
        static extern FFIResult _send_payment(
            ChannelManagerHandle handle,
            ref FFIRoute route,
            ref FFISha256dHash paymentHash,
            ref FFISecret paymentSecret
            );

        internal static FFIResult send_payment(
            ChannelManagerHandle handle,
            ref FFIRoute route,
            ref FFISha256dHash paymentHash,
            ref FFISecret paymentSecret,
            bool check = true
            )
        {
            return MaybeCheck(_send_payment(handle, ref route, ref paymentHash, ref paymentSecret), check);
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "funding_transaction_generated",
            ExactSpelling = true)]
        static extern FFIResult _funding_transaction_generated(IntPtr temporaryChannelId, FFIOutPoint fundingTxo, ChannelManagerHandle handle);

        internal static FFIResult funding_transaction_generated(IntPtr temporaryChannelId, FFIOutPoint fundingTxo,
            ChannelManagerHandle handle)
        {
            return MaybeCheck(_funding_transaction_generated(temporaryChannelId, fundingTxo, handle), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "process_pending_htlc_forwards",
            ExactSpelling = true)]
        static extern FFIResult _process_pending_htlc_forwards(ChannelManagerHandle handle);

        internal static FFIResult process_pending_htlc_forwards(ChannelManagerHandle handle)
        {
            return MaybeCheck(_process_pending_htlc_forwards(handle), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "timer_chan_freshness_every_min",
            ExactSpelling = true)]
        static extern FFIResult _timer_chan_freshness_every_min(ChannelManagerHandle handle);

        internal static FFIResult timer_chan_freshness_every_min(ChannelManagerHandle handle)
        {
            return MaybeCheck(_timer_chan_freshness_every_min(handle), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "fail_htlc_backwards",
            ExactSpelling = true)]
        static extern FFIResult _fail_htlc_backwards(ref FFISha256dHash paymentHash, ref FFISecret paymentSecret, ChannelManagerHandle handle, out byte result);

        internal static FFIResult fail_htlc_backwards(ref FFISha256dHash paymentHash, ref FFISecret paymentSecret, ChannelManagerHandle handle, out byte result)
        {
            return MaybeCheck(_fail_htlc_backwards(ref paymentHash, ref paymentSecret, handle, out result), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "fail_htlc_backwards_without_secret",
            ExactSpelling = true)]
        static extern FFIResult _fail_htlc_backwards_without_secret(ref FFISha256dHash paymentHash, ChannelManagerHandle handle, out byte result);

        internal static FFIResult fail_htlc_backwards_without_secret(ref FFISha256dHash paymentHash, ChannelManagerHandle handle, out byte result)
        {
            return MaybeCheck(_fail_htlc_backwards_without_secret(ref paymentHash, handle, out result), true);
        }
        
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "claim_funds",
            ExactSpelling = true)]
        static extern FFIResult _claim_funds(ref Array32 paymentPreimage, ref Array32 paymentSecret, ulong expectedAmount, ChannelManagerHandle handle, out byte result);

        internal static FFIResult claim_funds(ref Array32 paymentPreimage, ref Array32 paymentSecret, ulong expectedAmount, ChannelManagerHandle handle, out byte result)
        {
            return MaybeCheck(_claim_funds(ref paymentPreimage, ref paymentSecret, expectedAmount, handle, out result), true);
        }
        
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "get_and_clear_pending_events",
            ExactSpelling = true)]
        static extern FFIResult _get_and_clear_pending_events(ChannelManagerHandle handle, out FFIBytes events);

        internal static FFIResult get_and_clear_pending_events(ChannelManagerHandle handle, out FFIBytes events,
            bool check = true)
            => MaybeCheck(_get_and_clear_pending_events(handle, out events), check);
        

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "release_ffi_channel_manager",
            ExactSpelling = true)]
        static extern FFIResult _release_ffi_channel_manager(IntPtr chan_man);

        internal static FFIResult release_ffi_channel_manager(
            IntPtr chan_man,
            bool check = true
        ) => MaybeCheck(_release_ffi_channel_manager(chan_man), check);
    }
}