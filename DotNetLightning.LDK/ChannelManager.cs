using System;
using System.Threading;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Interfaces;
using DotNetLightning.LDK.Primitives;

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
            Network network,
            in UserConfig config,
            IManyChannelMonitor monitor,
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
                    var ffiManyChannelMonitor = monitor.ToFFI();
                    var ffiLogger = logger.ToFFI();
                    var ffiBroadcaster = broadcaster.ToFFI();
                    var ffiFeeEstimator = feeEstimator.ToFFI();
                    Interop.create_ffi_channel_manager(
                        b,
                        seed.Length,
                        in network,
                        in config,
                        in ffiManyChannelMonitor,
                        in ffiLogger,
                        in ffiBroadcaster,
                        in ffiFeeEstimator,
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