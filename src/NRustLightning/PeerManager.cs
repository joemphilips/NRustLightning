using System;
using System.IO;
using System.Threading;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.Interfaces;

namespace NRustLightning
{
    public class PeerManager : IDisposable
    {
        internal readonly PeerManagerHandle Handle;
        private readonly object[] _deps;
        private readonly Timer tick;
        
        internal PeerManager(
            PeerManagerHandle handle,
            int tickInterval,
            object[] deps
            )
        {
            _deps = deps;
            tick = new Timer(_ => Tick(), null, tickInterval, tickInterval);
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public static PeerManager Create(
            Span<byte> seed,
            in Network network,
            in UserConfig config,
            IChainWatchInterface chainWatchInterface,
            IBroadcaster broadcaster,
            ILogger logger,
            IFeeEstimator feeEstimator,
            IRoutingMsgHandler routingMsgHandler,
            uint currentBlockHeight,
            Span<byte> ourNodeSecret,
            int tickIntervalMSec = 30000
            )
        {
            if (seed.Length != 32) throw new InvalidDataException($"seed must be 32 bytes! it was {seed.Length}");
            if (ourNodeSecret.Length != 32) throw new InvalidDataException($"ourNodeSecret must be 32 bytes! it was {seed.Length}");
            var ourNodeId = new NBitcoin.Key(ourNodeSecret.ToArray()).PubKey.ToBytes();
            unsafe
            {
                fixed (byte* seedPtr = seed)
                fixed (Network* n = &network)
                fixed (UserConfig* configPtr = &config)
                fixed (byte* secretPtr = ourNodeSecret)
                fixed (byte* pubkeyPtr = ourNodeId)
                {
                    var ffiOurNodeSecret  = new FFISecretKey((IntPtr)secretPtr, (UIntPtr)ourNodeSecret.Length);
                    var ffiOurNodeId = new FFIPublicKey((IntPtr)pubkeyPtr, (UIntPtr)ourNodeId.Length);
                    Interop.create_peer_manager(
                        seedPtr,
                        (UIntPtr)(seed.Length),
                        n,
                        configPtr,
                        ref chainWatchInterface.InstallWatchTx,
                        ref chainWatchInterface.InstallWatchOutPoint,
                        ref chainWatchInterface.WatchAllTxn,
                        ref chainWatchInterface.GetChainUtxo,
                        ref broadcaster.BroadcastTransaction,
                        ref logger.Log,
                        ref feeEstimator.getEstSatPer1000Weight,
                        (UIntPtr)currentBlockHeight,
                        ref ffiOurNodeSecret,
                        ref ffiOurNodeId,
                        out var handle
                        );
                    return new PeerManager(handle, tickIntervalMSec,new object[]{ chainWatchInterface, broadcaster, logger, feeEstimator, routingMsgHandler });
                }
            }
        }
        
        private void Tick()
        {
            Interop.timer_tick_occured(Handle);
        }

        public void NewInboundConnection(ISocketDescriptor descriptor)
        {
            Interop.new_inbound_connection(descriptor.Index, ref descriptor.SendData, ref descriptor.DisconnectSocket, Handle);
        }

        public void NewOutboundConnection(ISocketDescriptor descriptor, FFIPublicKey theirNodeId)
        {
            
            Interop.new_outbound_connection(descriptor.Index, ref descriptor.SendData, ref descriptor.DisconnectSocket, ref theirNodeId, Handle);
        }
        public void WriteBufferSpaceAvail(ISocketDescriptor descriptor)
        {
            Interop.write_buffer_space_avail(descriptor.Index, ref descriptor.SendData, ref descriptor.DisconnectSocket, Handle);
        }

        public void SocketDisconnected(ISocketDescriptor descriptor)
        {
            Interop.socket_disconnected(descriptor.Index, ref descriptor.SendData, ref descriptor.DisconnectSocket, Handle);
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
                    descriptor.Index, ref descriptor.SendData, ref descriptor.DisconnectSocket, ref bytes, out var shouldPause, Handle);
                return shouldPause == 1;
            }
        }

        public void ProcessEvents()
        {
            Interop.process_events(Handle);
        }

        public void Dispose()
        {
            tick.Dispose();
            Handle.Dispose();
        }
    }
}