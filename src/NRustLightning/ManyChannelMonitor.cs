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
    public class ManyChannelMonitor : IDisposable, IChainListener
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
            return Create(new ChainWatchInterfaceDelegatesHolder(chainWatchInterface),
                new BroadcasterDelegatesHolder(broadcaster, network), new LoggerDelegatesHolder(logger),
                new FeeEstimatorDelegatesHolder(feeEstimator));
        }

        private static ManyChannelMonitor Create(
            in ChainWatchInterfaceDelegatesHolder chainWatchInterfaceDelegatesHolder,
            in BroadcasterDelegatesHolder broadcasterDelegatesHolder,
            in LoggerDelegatesHolder loggerDelegatesHolder,
            in FeeEstimatorDelegatesHolder feeEstimatorDelegatesHolder
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
                    var ffiResult = Interop.serialize_many_channel_monitor(bufOut, bufLength, out var actualLength, Handle, false);
                    return (ffiResult, actualLength);
                };

            return WithVariableLengthReturnBuffer(pool, func);
        }

        public static unsafe (ManyChannelMonitor, Dictionary<Primitives.LNOutPoint, uint256>) Deserialize(
            ManyChannelMonitorReadArgs readArgs,
            ReadOnlyMemory<byte> buffer,
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
                            readArgs.ChainWatchInterface.InstallWatchTx,
                            readArgs.ChainWatchInterface.InstallWatchOutPoint,
                            readArgs.ChainWatchInterface.WatchAllTxn,
                            readArgs.ChainWatchInterface.GetChainUtxo,
                            readArgs.ChainWatchInterface.FilterBlock,
                            readArgs.ChainWatchInterface.ReEntered,
                            readArgs.Broadcaster.BroadcastTransaction,
                            readArgs.Logger.Log,
                            readArgs.FeeEstimator.getEstSatPer1000Weight,
                            outputBufPtr, outputBufLen,
                            out var actualBufLen,
                            out newHandle
                        );
                        return (result, actualBufLen);
                    }
                };
            var buf = WithVariableLengthReturnBuffer(pool, func);
            var keyToBlockHash = ParseChannelMonitorKeyToItsLatestBlockHash(buf);
            return (
                new ManyChannelMonitor(newHandle, new object[] {readArgs}),
                keyToBlockHash
                );
        }

        public void TellBlockConnectedAfterResume(Block block, uint height, OutPoint key)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));
            if (key == null) throw new ArgumentNullException(nameof(key));
            var b = block.ToBytes();
            unsafe
            {
                fixed (byte* blockPtr = b)
                {
                    var outpoint = new FFIOutPoint(key);
                    Interop.tell_block_connected_after_resume((IntPtr)blockPtr, (UIntPtr)b.Length, height, ref outpoint, Handle);
                }
            }
        }

        public void TellBlockDisconnectedAfterResume(uint256 blockHeaderHash, uint height, OutPoint key)
        {
            if (blockHeaderHash == null) throw new ArgumentNullException(nameof(blockHeaderHash));
            if (key == null) throw new ArgumentNullException(nameof(key));
            var b = blockHeaderHash.ToBytes();
            unsafe
            {
                fixed (byte* bPtr = b)
                {
                    var outpoint = new FFIOutPoint(key);
                    Interop.tell_block_disconnected_after_resume((IntPtr) bPtr, height, ref outpoint, Handle);
                }
            }
        }

        public Event[] GetAndClearPendingEvents(MemoryPool<byte> pool)
        {
            if (pool == null) throw new ArgumentNullException(nameof(pool));
            FFIOperationWithVariableLengthReturnBuffer func =
                (bufOut, bufLength) =>
                {
                    var ffiResult =
                        Interop.many_channel_monitor_get_and_clear_pending_events(Handle, bufOut, bufLength,
                            out var actualBufLen, false);
                    return (ffiResult, actualBufLen);
                };
            return Event.ParseManyUnsafe(WithVariableLengthReturnBuffer(pool, func));
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
                foreach (var dep in _deps)
                {
                    if (dep is IDisposable d)
                        d.Dispose();
                }
                _disposed = true;
            }
        }

        public void BlockConnected(Block block, uint height, Primitives.LNOutPoint? key)
        {
            TellBlockConnectedAfterResume(block, height, key.Item);
        }

        public void BlockDisconnected(BlockHeader header, uint height, Primitives.LNOutPoint? key)
        {
            TellBlockDisconnectedAfterResume(header.GetHash(), height, key.Item);
        }
    }
}