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
            in Network n,
            UserConfig* config,
            
            ref InstallWatchTx installWatchTx,
            ref InstallWatchOutPoint installWatchOutPoint,
            ref WatchAllTxn watchAllTxn,
            ref GetChainUtxo getChainUtxo,
            ref FilterBlock filterBlock,
            ref ReEntered reEntered,
            
            ref GetNodeSecret getNodeSecret,
            ref GetDestinationScript getDestinationScript,
            ref GetShutdownKey getShutdownKey,
            ref GetChannelKeys getChannelKeys,
            ref GetOnionRand getOnionRand,
            ref GetChannelId getChannelId,
            
            ref BroadcastTransaction broadcastTransaction,
            ref Log log,
            ref GetEstSatPer1000Weight getEstSatPer1000Weight,
            ulong current_block_height,
            out ChannelManagerHandle handle
            );

        internal static unsafe FFIResult create_ffi_channel_manager(
            in Network n,
            UserConfig* config,
            
            InstallWatchTx installWatchTx,
            InstallWatchOutPoint installWatchOutPoint,
            WatchAllTxn watchAllTxn,
            GetChainUtxo getChainUtxo,
            FilterBlock filterBlock,
            ReEntered reEntered,
            
            GetNodeSecret getNodeSecret,
            GetDestinationScript getDestinationScript,
            GetShutdownKey getShutdownKey,
            GetChannelKeys getChannelKeys,
            GetOnionRand getOnionRand,
            GetChannelId getChannelId,
            
            BroadcastTransaction broadcastTransaction,
            Log log,
            GetEstSatPer1000Weight getEstSatPer1000Weight,
            ulong current_block_height,
            out ChannelManagerHandle handle,
            bool check = true
        )
        {
            return MaybeCheck(_create_ffi_channel_manager(n , config, ref installWatchTx, ref installWatchOutPoint, ref watchAllTxn, ref getChainUtxo, ref filterBlock, ref  reEntered,
                ref getNodeSecret, ref getDestinationScript, ref getShutdownKey, ref getChannelKeys, ref getOnionRand, ref getChannelId,
                ref broadcastTransaction,ref log, ref getEstSatPer1000Weight, current_block_height, out handle), check);
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
        static extern FFIResult _create_channel(IntPtr publicKey, ulong channelValueSatoshis, ulong pushMsat, ulong userId, ChannelManagerHandle handle);
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "create_channel_with_custom_config",
            ExactSpelling = true)]
        static extern FFIResult _create_channel_with_custom_config(IntPtr publicKey, ulong channelValueSatoshis, ulong pushMsat, ulong userId, in UserConfig config, ChannelManagerHandle handle);

        internal static FFIResult create_channel(IntPtr publicKey, ulong channelValueSatoshis, ulong pushMsat,
            ulong userId, ChannelManagerHandle handle, in UserConfig config, bool check = true)
        {
            return MaybeCheck(_create_channel_with_custom_config(publicKey, channelValueSatoshis, pushMsat, userId, in config, handle), check);
        }
        
        internal static FFIResult create_channel(IntPtr publicKey, ulong channelValueSatoshis, ulong pushMsat,
            ulong userId, ChannelManagerHandle handle, bool check = true)
        {
            return MaybeCheck(_create_channel(publicKey, channelValueSatoshis, pushMsat, userId, handle), check);
        }

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "close_channel",
            ExactSpelling = true)]
        static extern unsafe FFIResult _close_channel(IntPtr channelId, ChannelManagerHandle handle);
        internal static unsafe FFIResult close_channel(IntPtr channelId, ChannelManagerHandle handle)
        {
            return MaybeCheck(_close_channel(channelId, handle), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "foce_close_channel",
            ExactSpelling = true)]
        static extern FFIResult _force_close_channel(IntPtr channelId, ChannelManagerHandle handle);
        internal static FFIResult force_close_channel(IntPtr channelId, ChannelManagerHandle handle)
        {
            return MaybeCheck(_force_close_channel(channelId, handle), true);
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
            IntPtr paymentHash,
            IntPtr paymentSecret
            );

        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "send_payment_without_secret",
            ExactSpelling = true)]
        static extern FFIResult _send_payment_without_secret(
            ChannelManagerHandle handle,
            ref FFIRoute route,
            IntPtr paymentHash
            );
        internal static FFIResult send_payment(
            ChannelManagerHandle handle,
            ref FFIRoute route,
            IntPtr paymentHash,
            IntPtr? paymentSecret = null,
            bool check = true
            )
        {
            if (paymentSecret is null)
            {
                return MaybeCheck(_send_payment_without_secret(handle, ref route, paymentHash), check);
            }
            return MaybeCheck(_send_payment(handle, ref route, paymentHash, paymentSecret.Value), check);
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
        static extern FFIResult _fail_htlc_backwards(IntPtr paymentHash, IntPtr paymentSecret, ChannelManagerHandle handle, out byte result);

        internal static FFIResult fail_htlc_backwards(IntPtr paymentHash, IntPtr paymentSecret, ChannelManagerHandle handle, out byte result)
        {
            return MaybeCheck(_fail_htlc_backwards(paymentHash, paymentSecret, handle, out result), true);
        }
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "fail_htlc_backwards_without_secret",
            ExactSpelling = true)]
        static extern FFIResult _fail_htlc_backwards_without_secret(IntPtr paymentHash, ChannelManagerHandle handle, out byte result);

        internal static FFIResult fail_htlc_backwards_without_secret(IntPtr paymentHash, ChannelManagerHandle handle, out byte result)
        {
            return MaybeCheck(_fail_htlc_backwards_without_secret(paymentHash, handle, out result), true);
        }
        
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "claim_funds",
            ExactSpelling = true)]
        static extern FFIResult _claim_funds(IntPtr paymentPreimage, IntPtr paymentSecret, ulong expectedAmount, ChannelManagerHandle handle, out byte result);

        internal static FFIResult claim_funds(IntPtr paymentPreimage, IntPtr paymentSecret, ulong expectedAmount, ChannelManagerHandle handle, out byte result)
        {
            return MaybeCheck(_claim_funds(paymentPreimage, paymentSecret, expectedAmount, handle, out result), true);
        }
        
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "update_fee",
            ExactSpelling = true)]
        static extern FFIResult _update_fee(IntPtr channelId, uint feeratePerkw, ChannelManagerHandle handle);

        internal static FFIResult update_fee(IntPtr channelId, uint feeratePerKw, ChannelManagerHandle handle, bool check = true)
        {
            return MaybeCheck(_update_fee(channelId, feeratePerKw, handle), check);
        }
        
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "get_and_clear_pending_events",
            ExactSpelling = true)]
        static extern FFIResult _get_and_clear_pending_events(ChannelManagerHandle handle, IntPtr bufOut, UIntPtr bufLen, out UIntPtr actualBufLen);

        internal static FFIResult get_and_clear_pending_events(ChannelManagerHandle handle, IntPtr bufOut, UIntPtr bufLen, out UIntPtr actualBufLen,
            bool check = true)
            => MaybeCheck(_get_and_clear_pending_events(handle, bufOut, bufLen, out actualBufLen), check);
        
        [DllImport(RustLightning,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "serialize_channel_manager",
            ExactSpelling = true)]
        static extern FFIResult _serialize_channel_manager(IntPtr bufOut, UIntPtr bufLen, out UIntPtr actualBufLen, ChannelManagerHandle handle);

        internal static FFIResult serialize_channel_manager(IntPtr bufOut, UIntPtr bufLen, out UIntPtr actualBufLen, ChannelManagerHandle handle,
            bool check = true)
            => MaybeCheck(_serialize_channel_manager(bufOut, bufLen, out actualBufLen, handle), check);
        

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