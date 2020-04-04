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
        private readonly ChannelMonitorHandle _handle;
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
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }
        // when passing delegates from C# to rust, we don't have to pin it but
        // we must
        // 1. declare as a static field. (it may be unnecessary, I must check later.)
        // 2. make a copy of it
        // so that we can make sure that GC won't erase the pointer.
        // we e.g. https://stackoverflow.com/questions/5465060/do-i-need-to-pin-an-anonymous-delegate/5465074#5465074
        // and https://stackoverflow.com/questions/29300465/passing-function-pointer-in-c-sharp
        private static FFIBroadcastTransaction broadcast_ptr = (ref FFITransaction tx) =>
        {
            Console.WriteLine($"tx is {Hex.Encode(tx.AsSpan())}");
        };


        private static Log logConsole = (ref FFILogRecord record) =>
        {
            Console.WriteLine($"Logging {record.args}");
        };

        private static FFIGetEstSatPer1000Weight get_est_sat_per_1000_weight =
            (ref FFITransaction tx) =>
            {
                Console.WriteLine("Getting fee estimation");
                return 1234;
            };
        
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
            return new ChannelMonitor(chainWatchInterfaceHandle, broadcasterHandle, feeEstimatorHandle, loggerHandle, handle);
        }
        
        public void Dispose()
        {
            _handle.Dispose();
            _chainWatchInterfaceHandle.Dispose();
            _broadcasterHandle.Dispose();
            _feeEstimatorHandle.Dispose();
            _loggerHandle.Dispose();
        }
    }
}