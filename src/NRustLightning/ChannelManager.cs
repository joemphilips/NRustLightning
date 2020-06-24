using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Facades;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using NRustLightning.RustLightningTypes;
using NRustLightning.Utils;
using Network = NRustLightning.Adaptors.Network;
using static NRustLightning.Utils.Utils;
using Array = DotNetLightning.Utils.Array;

namespace NRustLightning
{
    
    public sealed class ChannelManager : IDisposable
    {
        internal readonly ChannelManagerHandle Handle;
        private bool _disposed = false;
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
            Errors.AssertDataLength(nameof(seed), seed.Length, 32);
            unsafe
            {
                fixed (byte* b = seed)
                fixed (Network* n = &network)
                fixed (UserConfig* configPtr = &config)
                {
                    Interop.create_ffi_channel_manager(
                        (IntPtr)b,
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

        public unsafe void CreateChannel(PubKey theirNetworkKey, ulong channelValueSatoshis, ulong pushMSat, ulong userId, in UserConfig overrideConfig)
        {
            if (theirNetworkKey == null) throw new ArgumentNullException(nameof(theirNetworkKey));
            if (!theirNetworkKey.IsCompressed) Errors.PubKeyNotCompressed(nameof(theirNetworkKey), theirNetworkKey);
            var pk = theirNetworkKey.ToBytes();
            fixed (byte* b = pk)
            fixed (UserConfig* _u = &overrideConfig)
            {
                Interop.create_channel((IntPtr) b, channelValueSatoshis, pushMSat, userId, Handle,
                    in overrideConfig);
            }
        }
        
        public unsafe void CreateChannel(PubKey theirNetworkKey, ulong channelValueSatoshis, ulong pushMSat, ulong userId)
        {
            if (theirNetworkKey == null) throw new ArgumentNullException(nameof(theirNetworkKey));
            if (!theirNetworkKey.IsCompressed) Errors.PubKeyNotCompressed(nameof(theirNetworkKey), theirNetworkKey);
            var pk = theirNetworkKey.ToBytes();
            fixed (byte* b = pk)
            {
                Interop.create_channel((IntPtr) b, channelValueSatoshis, pushMSat, userId, Handle);
            }
        }

        public unsafe void CloseChannel(uint256 channelId)
        {
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            var bytes = channelId.ToBytes();
            fixed (byte* b = bytes)
            {
                Interop.close_channel((IntPtr)b, Handle);
            }
        }

        public unsafe void ForceCloseChannel(uint256 channelId)
        {
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            var bytes = channelId.ToBytes();
            fixed (byte* b = bytes)
            {
                Interop.force_close_channel((IntPtr) b, Handle);
            }
        }

        public void ForceCloseAllChannels()
        {
            Interop.force_close_all_channels(Handle);
        }

        public void SendPayment(RoutesWithFeature routesWithFeature, Span<byte> paymentHash)
            => SendPayment(routesWithFeature, paymentHash, new byte[0]);
        
        public void SendPayment(RoutesWithFeature routesWithFeature, Span<byte> paymentHash, Span<byte> paymentSecret)
        {
            if (routesWithFeature == null) throw new ArgumentNullException(nameof(routesWithFeature));
            Errors.AssertDataLength(nameof(paymentHash), paymentHash.Length, 32);
            if (paymentSecret.Length != 0 && paymentSecret.Length != 32) throw new ArgumentException($"paymentSecret must be length of 32 or empty");
            unsafe
            {

                var routesInBytes = routesWithFeature.AsArray();
                fixed (byte* r = routesInBytes)
                fixed (byte* p = paymentHash)
                fixed (byte* s = paymentSecret)
                {
                    var route = new FFIRoute((IntPtr)r, (UIntPtr)routesInBytes.Length);
                    if (paymentSecret.Length == 32)
                        Interop.send_payment(Handle, ref route, (IntPtr)p, (IntPtr)s);
                    if (paymentSecret.Length == 0)
                    {
                        Interop.send_payment(Handle, ref route, (IntPtr)p);
                    }
                }
            }
        }

        public unsafe void FundingTransactionGenerated(Span<byte> temporaryChannelId, OutPoint fundingTxo)
        {
            if (fundingTxo == null) throw new ArgumentNullException(nameof(fundingTxo));
            Errors.AssertDataLength(nameof(temporaryChannelId), temporaryChannelId.Length, 32);

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

        public unsafe bool FailHTLCBackwards(Primitives.PaymentHash paymentHash, Primitives.PaymentPreimage paymentSecret = null)
        {
            if (paymentHash == null) throw new ArgumentNullException(nameof(paymentHash));
            var paymentHashBytes = paymentHash.Value.ToBytes();
            if (paymentSecret is null)
            {
                fixed (byte* p1 = paymentHashBytes)
                {
                    Interop.fail_htlc_backwards_without_secret((IntPtr) p1, Handle, out var result);
                    return result == 1;
                }
            }
            var paymentSecretBytes = paymentSecret.ToBytes().ToArray();
            fixed (byte* p1 = paymentHashBytes)
            fixed (byte* p2 = paymentSecretBytes)
            {
                Interop.fail_htlc_backwards((IntPtr) p1,(IntPtr)p2, Handle, out var result);
                return result == 1;
            }
        }

        public unsafe bool ClaimFunds(Primitives.PaymentPreimage paymentPreimage, uint256 paymentSecret, ulong expectedAmount)
        {
            if (paymentPreimage == null) throw new ArgumentNullException(nameof(paymentPreimage));
            if (paymentSecret == null) throw new ArgumentNullException(nameof(paymentSecret));
            var b1 = paymentPreimage.ToBytes().ToArray();
            var b2 = paymentSecret.ToBytes().ToArray();
            fixed(byte* p1 = b1)
            fixed (byte* p2 = b2)
            {
                Interop.claim_funds((IntPtr) p1, (IntPtr) p2, expectedAmount, Handle, out var result);
                return result == 1;
            }
        }

        public unsafe void UpdateFee(uint256 channelId, ulong feeRatePerKw)
        {
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            var b = channelId.ToBytes();
            fixed (byte* c = b)
            {
                Interop.update_fee((IntPtr)c, feeRatePerKw, Handle, true);
            }
        }
        
        public Event[] GetAndClearPendingEvents(MemoryPool<byte> pool)
        {
            Func<IntPtr, UIntPtr, ChannelManagerHandle, (FFIResult, UIntPtr)> func =
                (bufOut, bufLength, handle) =>
                {
                    var ffiResult = Interop.get_and_clear_pending_events(handle, bufOut, bufLength, out var actualLength);
                    return (ffiResult, actualLength);
                };
            var arr = WithVariableLengthReturnBuffer(pool, func, Handle);
            return Event.ParseManyUnsafe(arr);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Handle.Dispose();
                _disposed = true;
            }
        }
    }
}