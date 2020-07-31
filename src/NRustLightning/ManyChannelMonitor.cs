using System;
using System.Buffers;
using System.Collections.Generic;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using NRustLightning.RustLightningTypes;
using NRustLightning.Utils;
using static NRustLightning.Utils.Utils;

namespace NRustLightning
{
    public class ManyChannelMonitor : IDisposable
    {
        internal readonly ManyChannelMonitorHandle Handle;
        private readonly object[] _deps;

        internal ManyChannelMonitor(ManyChannelMonitorHandle handle, object[] deps)
        {
            Handle = handle;
            _deps = deps;
        }

        public static ManyChannelMonitor Create(
            NBitcoin.Network network,
            IChainWatchInterface chainWatchInterface,
            IBroadcaster broadcaster,
            ILogger logger,
            IFeeEstimator feeEstimator)
        {
            return Create(new ChainWatchInterfaceConverter(chainWatchInterface),
                new BroadcasterDelegatesHolder(broadcaster, network), new LoggerDelegatesHolder(logger),
                new FeeEstimatorDelegatesHolder(feeEstimator));
        }

        private static ManyChannelMonitor Create(
            IChainWatchInterfaceDelegatesHolder chainWatchInterfaceDelegatesHolder,
            IBroadcasterDelegatesHolder broadcasterDelegatesHolder,
            ILoggerDelegatesHolder loggerDelegatesHolder,
            IFeeEstimatorDelegatesHolder feeEstimatorDelegatesHolder
        )
        {
            Interop.create_many_channel_monitor(
                chainWatchInterfaceDelegatesHolder.InstallWatchTx,
                chainWatchInterfaceDelegatesHolder.InstallWatchOutPoint,
                chainWatchInterfaceDelegatesHolder.WatchAllTxn,
                chainWatchInterfaceDelegatesHolder.GetChainUtxo,
                chainWatchInterfaceDelegatesHolder.FilterBlock,
                chainWatchInterfaceDelegatesHolder.ReEntered,
                broadcasterDelegatesHolder.BroadcastTransaction,
                loggerDelegatesHolder.Log,
                feeEstimatorDelegatesHolder.getEstSatPer1000Weight,
                out var handle
            );
            
            return new ManyChannelMonitor(handle, new object[]{chainWatchInterfaceDelegatesHolder, broadcasterDelegatesHolder, loggerDelegatesHolder, feeEstimatorDelegatesHolder});
        }

        public byte[] Serialize(MemoryPool<byte> pool)
        {
            
            FFIOperationWithVariableLengthReturnBuffer func =
                (bufOut, bufLength) =>
                {
                    var ffiResult = Interop.serialize_many_channel_monitor(bufOut, bufLength, out var actualLength, Handle);
                    return (ffiResult, actualLength);
                };

            return WithVariableLengthReturnBuffer(pool, func);
        }

        public static (ManyChannelMonitor, Dictionary<Primitives.LNOutPoint, uint256>) Deserialize(
            NBitcoin.Network network,
            IChainWatchInterface chainWatchInterface,
            IBroadcaster broadcaster,
            ILogger logger,
            IFeeEstimator feeEstimator,
            Memory<byte> buffer,
            MemoryPool<byte> pool
        )
        {
            return Deserialize(new ChainWatchInterfaceConverter(chainWatchInterface),
                new BroadcasterDelegatesHolder(broadcaster, network), new LoggerDelegatesHolder(logger),
                new FeeEstimatorDelegatesHolder(feeEstimator), buffer, pool);
        }

        private static unsafe (ManyChannelMonitor, Dictionary<Primitives.LNOutPoint, uint256>) Deserialize(
            IChainWatchInterfaceDelegatesHolder chainWatchInterfaceDelegatesHolder,
            IBroadcasterDelegatesHolder broadcasterDelegatesHolder,
            ILoggerDelegatesHolder loggerDelegatesHolder,
            IFeeEstimatorDelegatesHolder feeEstimatorDelegatesHolder,
            Memory<byte> buffer,
            MemoryPool<byte> pool
        )
        {
            ManyChannelMonitorHandle newHandle = null;
            FFIOperationWithVariableLengthReturnBuffer func =
                (outputBufPtr, outputBufLen) =>
                {
                    fixed (byte* b = buffer.Span)
                    {
                        var result = Interop.deserialize_many_channel_monitor(
                            (IntPtr) b,
                            (UIntPtr) buffer.Length,
                            chainWatchInterfaceDelegatesHolder.InstallWatchTx,
                            chainWatchInterfaceDelegatesHolder.InstallWatchOutPoint,
                            chainWatchInterfaceDelegatesHolder.WatchAllTxn,
                            chainWatchInterfaceDelegatesHolder.GetChainUtxo,
                            chainWatchInterfaceDelegatesHolder.FilterBlock,
                            chainWatchInterfaceDelegatesHolder.ReEntered,
                            broadcasterDelegatesHolder.BroadcastTransaction,
                            loggerDelegatesHolder.Log,
                            feeEstimatorDelegatesHolder.getEstSatPer1000Weight,
                            outputBufPtr, outputBufLen,
                            out var actualBufLen,
                            out newHandle
                        );
                        return (result, actualBufLen);
                    }
                };
            var buf = WithVariableLengthReturnBuffer(pool, func);
            Console.WriteLine($"buffered key to blockhash is {Hex.Encode(buf)}");
            var keyToBlockHash = ParseChannelMonitorKeyToItsLatestBlockHash(buf);
            return (
                new ManyChannelMonitor(newHandle, new object[] {chainWatchInterfaceDelegatesHolder, broadcasterDelegatesHolder, loggerDelegatesHolder, feeEstimatorDelegatesHolder}),
                keyToBlockHash
                );
        }

        private static Dictionary<Primitives.LNOutPoint, uint256> ParseChannelMonitorKeyToItsLatestBlockHash(byte[] b)
        {
            return Parsers.ParseOutPointToBlockHashMap(b);
        }

        private bool _disposed = false;
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