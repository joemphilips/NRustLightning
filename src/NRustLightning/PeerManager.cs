using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using NRustLightning.Utils;
using static NRustLightning.Utils.Utils;
using Network = NRustLightning.Adaptors.Network;

namespace NRustLightning
{
    public class PeerManager : IDisposable
    {
        private readonly PeerManagerHandle _handle;
        private readonly object[] _deps;
        private readonly Timer tick;
        private bool _disposed = false;

        public ChannelManager ChannelManager { get; }
        public BlockNotifier BlockNotifier { get; }
        
        internal PeerManager(
            PeerManagerHandle handle,
            ChannelManager channelManager,
            BlockNotifier blockNotifier,
            int tickInterval,
            object[] deps
            )
        {
            _deps = deps;
            tick = new Timer(_ => Tick(), null, tickInterval, tickInterval);
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
            ChannelManager = channelManager ?? throw new ArgumentNullException(nameof(channelManager));
            BlockNotifier = blockNotifier ?? throw new ArgumentNullException(nameof(blockNotifier));
        }

        public static PeerManager Create(
            Span<byte> seed,
            NBitcoin.Network nbitcoinNetwork,
            in UserConfig config,
            IChainWatchInterface chainWatchInterface,
            IBroadcaster broadcaster,
            ILogger logger,
            IFeeEstimator feeEstimator,
            uint currentBlockHeight,
            Span<byte> ourNodeSecret,
            int tickIntervalMSec = 30000
            )
        {
            if (seed.Length != 32) throw new InvalidDataException($"seed must be 32 bytes! it was {seed.Length}");
            if (ourNodeSecret.Length != 32) throw new InvalidDataException($"ourNodeSecret must be 32 bytes! it was {seed.Length}");
            var ourNodeId = new NBitcoin.Key(ourNodeSecret.ToArray()).PubKey.ToBytes();
            var network = nbitcoinNetwork.ToFFINetwork();
            
            var chainWatchInterfaceDelegatesHolder = new ChainWatchInterfaceConverter(chainWatchInterface);
            var broadcasterDelegatesHolder = new BroadcasterDelegatesHolder(broadcaster, nbitcoinNetwork);
            var loggerDelegatesHolder = new LoggerDelegatesHolder(logger);
            var feeEstimatorDelegatesHolder = new FeeEstimatorDelegatesHolder(feeEstimator);
            
            var chanMan = ChannelManager.Create(seed, nbitcoinNetwork, in config, chainWatchInterfaceDelegatesHolder, loggerDelegatesHolder, broadcasterDelegatesHolder, feeEstimatorDelegatesHolder, currentBlockHeight);
            var blockNotifier = BlockNotifier.Create(nbitcoinNetwork, loggerDelegatesHolder, chainWatchInterfaceDelegatesHolder);
            blockNotifier.RegisterChannelManager(chanMan);
            unsafe
            {
                fixed (byte* seedPtr = seed)
                fixed (UserConfig* configPtr = &config)
                fixed (byte* secretPtr = ourNodeSecret)
                fixed (byte* pubkeyPtr = ourNodeId)
                {
                    Interop.create_peer_manager(
                        (IntPtr)seedPtr,
                        in network,
                        configPtr,
                        chanMan.Handle,
                        chainWatchInterfaceDelegatesHolder.InstallWatchTx,
                        chainWatchInterfaceDelegatesHolder.InstallWatchOutPoint,
                        chainWatchInterfaceDelegatesHolder.WatchAllTxn,
                        chainWatchInterfaceDelegatesHolder.GetChainUtxo,
                        chainWatchInterfaceDelegatesHolder.FilterBlock,
                        chainWatchInterfaceDelegatesHolder.ReEntered,
                        loggerDelegatesHolder.Log,
                        (IntPtr)secretPtr,
                        (IntPtr)pubkeyPtr,
                        out var handle
                        );
                    return new PeerManager(handle, chanMan, blockNotifier, tickIntervalMSec,new object[]{ chainWatchInterfaceDelegatesHolder, broadcasterDelegatesHolder, loggerDelegatesHolder, feeEstimatorDelegatesHolder, });
                }
            }
        }
        
        private void Tick()
        {
            Interop.timer_tick_occured(_handle);
            ChannelManager.TimerChanFreshnessEveryMin();
        }

        public void NewInboundConnection(ISocketDescriptor descriptor)
        {
            Interop.new_inbound_connection(descriptor.Index, descriptor.SendData, descriptor.DisconnectSocket, _handle);
        }

        /// <summary>
        /// Returns noise act one (50 bytes)
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="theirNodeId"></param>
        public unsafe byte[] NewOutboundConnection(ISocketDescriptor descriptor, Span<byte> theirNodeId)
        {
            fixed (byte* p = theirNodeId)
            {
                Interop.new_outbound_connection(descriptor.Index, descriptor.SendData,
                    descriptor.DisconnectSocket, (IntPtr)p, _handle, out var initialSend);
                return initialSend.AsArray();
            }
        }
        public void WriteBufferSpaceAvail(ISocketDescriptor descriptor)
        {
            Interop.write_buffer_space_avail(descriptor.Index, descriptor.SendData, descriptor.DisconnectSocket, _handle);
        }

        public void SocketDisconnected(ISocketDescriptor descriptor)
        {
            Interop.socket_disconnected(descriptor.Index, descriptor.SendData, descriptor.DisconnectSocket, _handle);
        }

        /// <summary>
        /// If it returns `true`, we must stop feeding bytes into PeerManager for DoS prevention.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public unsafe bool ReadEvent(ISocketDescriptor descriptor, ReadOnlySpan<byte> data)
        {
            fixed (byte* d = data)
            {
                var bytes = new FFIBytes((IntPtr)d, (UIntPtr)data.Length);
                Interop.read_event(
                    descriptor.Index, descriptor.SendData, descriptor.DisconnectSocket, ref bytes, out var shouldPause, _handle);
                return shouldPause == 1;
            }
        }

        public unsafe bool TryReadEvent(ISocketDescriptor descriptor, ReadOnlySpan<byte> data, out bool shouldPause,
            out FFIResult result)
        {
            fixed (byte* d = data)
            {
                var bytes = new FFIBytes((IntPtr)d, (UIntPtr)data.Length);
                result =
                Interop.read_event(
                    descriptor.Index, descriptor.SendData, descriptor.DisconnectSocket, ref bytes, out var shouldPauseB, _handle, false); 
                shouldPause = shouldPauseB == 1;
            }
            return result.IsSuccess;
        }

        public void ProcessEvents()
        {
            Interop.process_events(_handle);
        }

        public unsafe PubKey[] GetPeerNodeIds(MemoryPool<byte> pool)
        {
            var currentBufferSize = BUFFER_SIZE_UNIT;

            while (true)
            {
                using var memoryOwner = pool.Rent(currentBufferSize);
                var span = memoryOwner.Memory.Span;
                fixed (byte* ptr = span)
                {
                    var result = Interop.get_peer_node_ids(ptr, (UIntPtr) span.Length, out var actualNodeIdsLength,
                        _handle);
                    if ((int)actualNodeIdsLength > MAX_BUFFER_SIZE)
                    {
                        throw new FFIException(
                            $"Tried to return too long buffer form rust {currentBufferSize}. This should never happen.",
                            result);
                    }

                    if (result.IsSuccess)
                    {
                        var arr = span.Slice(0, (int)actualNodeIdsLength).ToArray();
                        return DecodePubKeyArray(arr);
                    }

                    if (result.IsBufferTooSmall)
                    {
                        currentBufferSize = (int)actualNodeIdsLength;
                        continue;
                    }

                    result.Check();
                }
            }
        }

        private static PubKey[] DecodePubKeyArray(byte[] arr)
        {
            var nPubKey = arr[0..2].ToUInt16BE();
            var result = new PubKey[nPubKey];
            arr = arr[2..];
            Debug.Assert(nPubKey * 33 == arr.Length, $"Length must be multiple of 33. it was {arr.Length}. but length was {nPubKey}");
            for (int i = 0; i < nPubKey; i++)
            {
                var s = arr[(i * 33)..((i + 1) * 33)];
                result[i] = new PubKey(s);
            }

            return result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                tick.Dispose();
                ChannelManager.Dispose();
                BlockNotifier.Dispose();
                _handle.Dispose();
                _disposed = true;
            }
        }
    }
}