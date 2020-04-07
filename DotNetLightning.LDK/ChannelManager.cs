using System;
using System.Threading;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Interfaces;
using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK
{
    
    public sealed class ChannelManager : IDisposable
    {
        private readonly ChannelManagerHandle _handle;
        internal ChannelManager(ChannelManagerHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
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
                {
                    Interop.create_ffi_channel_manager(
                        b,
                        (UIntPtr)seed.Length,
                        in network,
                        in config,
                        ref chainWatchInterface.InstallWatchTx,
                        ref chainWatchInterface.InstallWatchOutPoint,
                        ref chainWatchInterface.WatchAllTxn,
                        ref chainWatchInterface.GetChainUtxo,
                        ref chainWatchInterface.FilterBlock,
                        ref broadcaster.BroadcastTransaction,
                        ref logger.Log,
                        ref feeEstimator.getEstSatPer1000Weight,
                        currentBlockHeight,
                        out var handle);
                    return new ChannelManager(handle);
                }
            }
        }
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}