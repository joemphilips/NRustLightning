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
                    var monitor = ChannelMonitor.Create(chainWatchInterface, broadcaster, logger, feeEstimator);
                    Interop.create_broadcaster(ref broadcaster.BroadcastTransaction, out var broadcasterHandle);
                    Interop.create_logger(ref logger.Log, out var loggerHandle);
                    Interop.create_fee_estimator(ref feeEstimator.getEstSatPer1000Weight, out var feeEstimatorHandle);
                    Interop.create_ffi_channel_manager(
                        b,
                        (UIntPtr)seed.Length,
                        in network,
                        in config,
                        monitor.Handle,
                        loggerHandle,
                        broadcasterHandle,
                        feeEstimatorHandle,
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