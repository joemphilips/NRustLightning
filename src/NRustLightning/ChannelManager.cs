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
using RustLightningTypes;
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
        internal ChannelManager(ChannelManagerHandle handle, object[]? deps = null)
        {
            _deps = deps ?? new object[] {};
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public static ChannelManager Create(
            NBitcoin.Network nbitcoinNetwork,
            IUserConfigProvider config,
            IChainWatchInterface chainWatchInterface,
            IKeysInterface keysInterface,
            ILogger logger,
            IBroadcaster broadcaster,
            IFeeEstimator feeEstimator,
            ulong currentBlockHeight,
            ManyChannelMonitor manyChannelMonitor
        )
        {
            var c = config.GetUserConfig();
            return Create(nbitcoinNetwork, in c, chainWatchInterface, keysInterface, logger, broadcaster, feeEstimator, currentBlockHeight, manyChannelMonitor);
        }

        public static ChannelManager Create(
            NBitcoin.Network nbitcoinNetwork,
            in UserConfig config,
            IChainWatchInterface chainWatchInterface,
            IKeysInterface keysInterface,
            ILogger logger,
            IBroadcaster broadcaster,
            IFeeEstimator feeEstimator,
            ulong currentBlockHeight,
            ManyChannelMonitor manyChannelMonitor
        )
        {
            
            var chainWatchInterfaceDelegatesHolder = new ChainWatchInterfaceConverter(chainWatchInterface);
            var keysInterfaceDelegatesHolder = new KeysInterfaceDelegatesHolder(keysInterface);
            var loggerDelegatesHolder = new LoggerDelegatesHolder(logger);
            var broadcasterDelegatesHolder = new BroadcasterDelegatesHolder(broadcaster, nbitcoinNetwork);
            var feeEstimatorDelegatesHolder = new FeeEstimatorDelegatesHolder(feeEstimator);
            return Create(
                nbitcoinNetwork,
                in config,
                chainWatchInterfaceDelegatesHolder,
                keysInterfaceDelegatesHolder,
                loggerDelegatesHolder,
                broadcasterDelegatesHolder,
                feeEstimatorDelegatesHolder,
                currentBlockHeight,
                manyChannelMonitor
                );
        }
        internal static ChannelManager Create(
            NBitcoin.Network nbitcoinNetwork,
            in UserConfig config,
            IChainWatchInterfaceDelegatesHolder chainWatchInterfaceDelegatesHolder,
            KeysInterfaceDelegatesHolder keysInterfaceDelegatesHolder,
            ILoggerDelegatesHolder loggerDelegatesHolder,
            IBroadcasterDelegatesHolder broadcasterDelegatesHolder,
            IFeeEstimatorDelegatesHolder feeEstimatorDelegatesHolder,
            ulong currentBlockHeight,
            ManyChannelMonitor manyChannelMonitor
            )
        {
            var n = nbitcoinNetwork.ToFFINetwork();
            unsafe
            {
                fixed (UserConfig* configPtr = &config)
                {
                    Interop.create_ffi_channel_manager(
                        in n,
                        configPtr,
                        chainWatchInterfaceDelegatesHolder.InstallWatchTx,
                        chainWatchInterfaceDelegatesHolder.InstallWatchOutPoint,
                        chainWatchInterfaceDelegatesHolder.WatchAllTxn,
                        chainWatchInterfaceDelegatesHolder.GetChainUtxo,
                        chainWatchInterfaceDelegatesHolder.FilterBlock,
                        chainWatchInterfaceDelegatesHolder.ReEntered,
                        
                        keysInterfaceDelegatesHolder.GetNodeSecret,
                        keysInterfaceDelegatesHolder.GetDestinationScript,
                        keysInterfaceDelegatesHolder.GetShutdownKey,
                        keysInterfaceDelegatesHolder.GetChannelKeys,
                        keysInterfaceDelegatesHolder.GetOnionRand,
                        keysInterfaceDelegatesHolder.GetChannelId,
                        
                        broadcasterDelegatesHolder.BroadcastTransaction,
                        loggerDelegatesHolder.Log,
                        feeEstimatorDelegatesHolder.getEstSatPer1000Weight,
                        currentBlockHeight,
                        manyChannelMonitor.Handle,
                        out var handle);
                    return new ChannelManager(handle, new object[] {chainWatchInterfaceDelegatesHolder, keysInterfaceDelegatesHolder, loggerDelegatesHolder, broadcasterDelegatesHolder, feeEstimatorDelegatesHolder});
                }
            }
        }

        public ChannelDetails[] ListChannels(MemoryPool<byte> pool)
        {
            FFIOperationWithVariableLengthReturnBuffer func =
                (bufOut, bufLength) =>
                {
                    var ffiResult = Interop.list_channels(bufOut, bufLength, out var actualChannelsLen, Handle, false);
                    return (ffiResult, actualChannelsLen);
                };

            var arr = WithVariableLengthReturnBuffer(pool, func);
            return ChannelDetails.ParseManyUnsafe(arr);
        }

        public void CreateChannel(PubKey theirNetworkKey, ulong channelValueSatoshis, ulong pushMSat, ulong userId,
            IUserConfigProvider overrideConfig)
        {
            var c = overrideConfig.GetUserConfig();
            CreateChannel(theirNetworkKey, channelValueSatoshis, pushMSat, userId, in c);
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
            var bytes = channelId.ToBytes(false);
            fixed (byte* b = bytes)
            {
                Interop.close_channel((IntPtr)b, Handle);
            }
        }

        public unsafe void ForceCloseChannel(uint256 channelId)
        {
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            var bytes = channelId.ToBytes(false);
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routesWithFeature"></param>
        /// <param name="paymentHash"></param>
        /// <param name="paymentSecret"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="PaymentSendException"></exception>
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
                    else
                    {
                        throw new Exception("Unreachable");
                    }
                }
            }
        }

        public void GetRouteAndSendPayment(NetworkGraph graph, PubKey theirNodeId, Primitives.PaymentHash paymentHash,
            IList<RouteHint> lastHops, LNMoney valueToSend, Primitives.BlockHeightOffset32 finalCLTV,
            uint256? paymentSecret = null)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            GetRouteAndSendPayment(graph.ToBytes(), theirNodeId, paymentHash, lastHops, valueToSend, finalCLTV, paymentSecret);
        }

        internal void GetRouteAndSendPayment(byte[] graphBytes, PubKey theirNodeId, Primitives.PaymentHash paymentHash, IList<RouteHint> lastHops, LNMoney valueToSend, Primitives.BlockHeightOffset32 finalCLTV, uint256? paymentSecret = null)
        {
            if (theirNodeId == null) throw new ArgumentNullException(nameof(theirNodeId));
            if (lastHops == null) throw new ArgumentNullException(nameof(lastHops));
            if (valueToSend.Value <= 0) throw new ArgumentException("value must be positive");
            if (finalCLTV.Value <= 0) throw new ArgumentException("value must be positive");
            var pkBytes = theirNodeId.ToBytes();
            var paymentHashBytes = paymentHash.ToBytes(false);
            var lastHopsBytes = lastHops.ToBytes();
            unsafe
            {
                fixed (byte* graphPtr = graphBytes)
                fixed (byte* pkPtr = pkBytes)
                fixed (byte* paymentHashPtr = paymentHashBytes)
                fixed (byte* lastHopsPtr = lastHopsBytes)
                {
                    var ffiLastHops = new FFIBytes((IntPtr) lastHopsPtr, (UIntPtr) lastHopsBytes.Length);
                    if (paymentSecret is null)
                    {
                        Interop.get_route_and_send_payment(graphPtr, (UIntPtr) graphBytes.Length, pkPtr,
                            ref ffiLastHops, (ulong)valueToSend.MilliSatoshi, finalCLTV.Value, paymentHashPtr, Handle, null, true);
                    }
                    else
                    {
                        fixed (byte* paymentSecretPtr = paymentSecret.ToBytes(false))
                        {
                            Interop.get_route_and_send_payment(graphPtr, (UIntPtr) graphBytes.Length, pkPtr,
                                ref ffiLastHops, (ulong)valueToSend.MilliSatoshi, finalCLTV.Value, paymentHashPtr, Handle, (IntPtr)paymentSecretPtr, true);
                        }
                    }
                }
            }
        }

        public unsafe void FundingTransactionGenerated(uint256 temporaryChannelId, OutPoint fundingTxo)
        {
            var temporaryChannelIdBytes = temporaryChannelId.ToBytes(false);
            if (fundingTxo == null) throw new ArgumentNullException(nameof(fundingTxo));
            Errors.AssertDataLength(nameof(temporaryChannelId), temporaryChannelIdBytes.Length, 32);

            fixed (byte* temporaryChannelIdPtr = temporaryChannelIdBytes)
            {
                var ffiOutPoint = new FFIOutPoint(fundingTxo);
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

        public unsafe bool FailHTLCBackwards(Primitives.PaymentHash paymentHash, uint256? paymentSecret = null)
        {
            if (paymentHash == null) throw new ArgumentNullException(nameof(paymentHash));
            var paymentHashBytes = paymentHash.ToBytes(false);
            if (paymentSecret is null)
            {
                fixed (byte* p1 = paymentHashBytes)
                {
                    Interop.fail_htlc_backwards_without_secret((IntPtr) p1, Handle, out var result);
                    return result == 1;
                }
            }
            var paymentSecretBytes = paymentSecret.ToBytes(false).ToArray();
            fixed (byte* p1 = paymentHashBytes)
            fixed (byte* p2 = paymentSecretBytes)
            {
                Interop.fail_htlc_backwards((IntPtr) p1,(IntPtr)p2, Handle, out var result);
                return result == 1;
            }
        }

        public unsafe bool ClaimFunds(Primitives.PaymentPreimage paymentPreimage, uint256? paymentSecret, ulong expectedAmount)
        {
            if (paymentPreimage == null) throw new ArgumentNullException(nameof(paymentPreimage));
            var b1 = paymentPreimage.ToBytes().ToArray();
            if (paymentSecret != null)
            {
                var b2 = paymentSecret.ToBytes(false).ToArray();
                fixed (byte* p1 = b1)
                fixed (byte* p2 = b2)
                {
                    Interop.claim_funds((IntPtr) p1, (IntPtr) p2, expectedAmount, Handle, out var result);
                    return result == 1;
                }
            }
            else
            {
                fixed (byte* p1 = b1)
                {
                    Interop.claim_funds_without_secret((IntPtr) p1, expectedAmount, Handle, out var result);
                    return result == 1;
                }
            }
        }

        public unsafe void UpdateFee(uint256 channelId, uint feeRatePerKw)
        {
            if (channelId == null) throw new ArgumentNullException(nameof(channelId));
            var b = channelId.ToBytes(false);
            fixed (byte* c = b)
            {
                Interop.update_fee((IntPtr)c, feeRatePerKw, Handle, true);
            }
        }

        public unsafe byte[] Serialize(MemoryPool<byte> pool)
        {
            FFIOperationWithVariableLengthReturnBuffer func =
                (bufOut, bufLength) =>
                {
                    var ffiResult = Interop.serialize_channel_manager(bufOut, bufLength, out var actualLength, Handle, false);
                    return (ffiResult, actualLength);
                };

            return WithVariableLengthReturnBuffer(pool, func);
        }

        public static ChannelManager Deserialize(ReadOnlySpan<byte> bytes, ChannelManagerReadArgs readArgs)
        {
            throw new NotImplementedException();
        }
        
        public Event[] GetAndClearPendingEvents(MemoryPool<byte> pool)
        {
            FFIOperationWithVariableLengthReturnBuffer func =
                (bufOut, bufLength) =>
                {
                    var ffiResult = Interop.get_and_clear_pending_events(Handle, bufOut, bufLength, out var actualLength ,false);
                    return (ffiResult, actualLength);
                };
            var arr = WithVariableLengthReturnBuffer(pool, func);
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