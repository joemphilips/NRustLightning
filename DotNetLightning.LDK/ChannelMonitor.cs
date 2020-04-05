using System;
using System.Runtime.InteropServices;
using System.Text;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Interfaces;
using DotNetLightning.LDK.Utils;

namespace DotNetLightning.LDK
{
    public sealed class ChannelMonitor : IDisposable
    {
        private readonly BroadcasterHandle _broadcasterHandle;
        private readonly ChainWatchInterfaceHandle _chainWatchInterfaceHandle;
        private readonly FeeEstimatorHandle _feeEstimatorHandle;
        private readonly LoggerHandle _loggerHandle;
        internal readonly ChannelMonitorHandle Handle;
        
        internal ChannelMonitor(
            ChainWatchInterfaceHandle chainWatchInterfaceHandle,
            BroadcasterHandle broadcasterHandle,
            FeeEstimatorHandle feeEstimatorHandle,
            LoggerHandle loggerHandle,
            ChannelMonitorHandle handle
            )
        {
            _chainWatchInterfaceHandle = chainWatchInterfaceHandle ?? throw new ArgumentNullException(nameof(chainWatchInterfaceHandle));
            _broadcasterHandle = broadcasterHandle ?? throw new ArgumentNullException(nameof(broadcasterHandle));
            _feeEstimatorHandle = feeEstimatorHandle ?? throw new ArgumentNullException(nameof(feeEstimatorHandle));
            _loggerHandle = loggerHandle ?? throw new ArgumentNullException(nameof(loggerHandle));
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }
        public static ChannelMonitor Create(
            IChainWatchInterface chainWatchInterface,
            IBroadcaster broadcaster,
            ILogger logger,
            IFeeEstimator feeEstimator
            )
        {
            Interop.create_chain_watch_interface(
                ref chainWatchInterface.InstallWatchTx, 
                ref chainWatchInterface.InstallWatchOutPoint, 
                ref chainWatchInterface.WatchAllTxn, 
                ref chainWatchInterface.GetChainUtxo, 
                ref chainWatchInterface.FilterBlock,
                out var chainWatchInterfaceHandle);
            Interop.create_broadcaster(ref broadcaster.BroadcastTransaction, out var broadcasterHandle);
            Interop.create_logger(ref logger.Log, out var loggerHandle);
            Interop.create_fee_estimator(ref feeEstimator.getEstSatPer1000Weight, out var feeEstimatorHandle);
            
            Interop.create_ffi_channel_monitor(chainWatchInterfaceHandle, broadcasterHandle, loggerHandle, feeEstimatorHandle, out var handle);
            return new ChannelMonitor(chainWatchInterfaceHandle, broadcasterHandle, feeEstimatorHandle, loggerHandle,
                handle);
        }
        
        public void Dispose()
        {
            Handle.Dispose();
            _chainWatchInterfaceHandle.Dispose();
            _broadcasterHandle.Dispose();
            _feeEstimatorHandle.Dispose();
            _loggerHandle.Dispose();
        }
    }
}