using System;
using System.Buffers;
using System.IO;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Facades;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using NRustLightning.NRustLightningTypes;
using NRustLightning.RustLightningTypes;
using NRustLightning.Utils;
using Network = NRustLightning.Adaptors.Network;
using static NRustLightning.Utils.Utils;

namespace NRustLightning
{
    
    public sealed class ChannelManager : IDisposable
    {
        internal readonly ChannelManagerHandle Handle;
        private readonly object[] _deps;
        internal ChannelManager(ChannelManagerHandle handle, object[] deps = null)
        {
            _deps = deps ?? new object[] {};
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }
        
        public static ChannelManager Create(
            Span<byte> seed,
            in Network network,
            in UserConfig config,
            IChainWatchInterface chainWatchInterface,
            ILogger logger,
            IBroadcaster broadcaster,
            IFeeEstimator feeEstimator,
            ulong currentBlockHeight
            )
        {
            unsafe
            {
                fixed (byte* b = seed)
                fixed (Network* n = &network)
                fixed (UserConfig* configPtr = &config)
                {
                    Interop.create_ffi_channel_manager(
                        b,
                        (UIntPtr)seed.Length,
                        n,
                        configPtr,
                        ref chainWatchInterface.InstallWatchTx,
                        ref chainWatchInterface.InstallWatchOutPoint,
                        ref chainWatchInterface.WatchAllTxn,
                        ref chainWatchInterface.GetChainUtxo,
                        ref broadcaster.BroadcastTransaction,
                        ref logger.Log,
                        ref feeEstimator.getEstSatPer1000Weight,
                        currentBlockHeight,
                        out var handle);
                    return new ChannelManager(handle, new object[] {chainWatchInterface, logger, broadcaster, feeEstimator});
                }
            }
        }

        public ChannelDetails[] ListChannels(MemoryPool<byte> pool)
        {
            Func<IntPtr, UIntPtr, ChannelManagerHandle, (FFIResult, UIntPtr)> func =
                (bufOut, bufLength, handle) =>
                {
                    var ffiResult = Interop.list_channels(bufOut, bufLength, out var actualChannelsLen, handle);
                    return (ffiResult, actualChannelsLen);
                };

            var arr = WithVariableLengthReturnBuffer(pool, func, Handle);
            return ChannelDetails.ParseManyUnsafe(arr);
        }

        public unsafe void CreateChannel(PubKey theirNetworkKey, ulong channelValueSatoshis, ulong pushMSat, ulong userId, in UserConfig? overrideConfig = null)
        {
            if (theirNetworkKey == null) throw new ArgumentNullException(nameof(theirNetworkKey));
            if (!theirNetworkKey.IsCompressed) Errors.PubKeyNotCompressed(nameof(theirNetworkKey), theirNetworkKey);
            var pk = theirNetworkKey.ToBytes();
            fixed (byte* b = pk)
            {
                var ffiPk = new FFIPublicKey((IntPtr)b, (UIntPtr)pk.Length);
                Interop.create_channel(ffiPk, channelValueSatoshis, pushMSat, userId, Handle, in overrideConfig);
            }
        }
        public unsafe void CloseChannel(uint256 channelId)
        {
            var bytes = channelId.ToBytes();
            fixed (byte* b = bytes)
            {
                Interop.close_channel(b, Handle);
            }
        }

        public void SendPayment(RoutesWithFeature routesWithFeature, Span<byte> paymentHash)
            => SendPayment(routesWithFeature, paymentHash, new byte[0]);
        
        public void SendPayment(RoutesWithFeature routesWithFeature, Span<byte> paymentHash, Span<byte> paymentSecret)
        {
            if (paymentSecret.Length != 0 && paymentSecret.Length != 32) throw new ArgumentException($"paymentSecret must be length of 32 or empty");
            unsafe
            {

                var routesInBytes = routesWithFeature.AsArray();
                fixed (byte* r = routesInBytes)
                fixed (byte* p = paymentHash)
                fixed (byte* s = paymentSecret)
                {
                    var route = new FFIRoute((IntPtr)r, (UIntPtr)routesInBytes.Length);
                    var ffiPaymentHash = new FFISha256dHash((IntPtr)p, (UIntPtr)paymentHash.Length);
                    var ffiPaymentSecret = new FFISecret((IntPtr)s, (UIntPtr)paymentSecret.Length);
                    Interop.send_payment(Handle, ref route, ref ffiPaymentHash, ref ffiPaymentSecret);
                }
            }
        }

        public Event[] GetAndClearPendingEvents()
        {
            Interop.get_and_clear_pending_events(Handle, out var eventsBytes);
            return Event.ParseManyUnsafe(eventsBytes.AsArray());
        }

        public unsafe void FundingTransactionGenerated(Span<byte> temporaryChannelId, OutPoint fundingTxo)
        {
            if (temporaryChannelId.Length != 32) throw new InvalidDataException($"length for {nameof(temporaryChannelId)} must be 32! it was {temporaryChannelId.Length}");

            fixed (byte* temporaryChannelIdPtr = temporaryChannelId)
            {
                var ffiOutPoint = new FFIOutPoint(fundingTxo.Hash, (ushort)fundingTxo.N);
                Interop.funding_transaction_generated((IntPtr)temporaryChannelIdPtr, ffiOutPoint, Handle);
            }
        }

        public void ProcessPendingHTLCForwards()
        {
            Interop.process_pending_htlc_forwards(Handle);
        }

        public void TimerChanFreshnessEveryMin()
        {
            Interop.timer_chan_freshness_every_min(Handle);
        }
        
        public void Dispose()
        {
            Handle.Dispose();
        }
    }
}