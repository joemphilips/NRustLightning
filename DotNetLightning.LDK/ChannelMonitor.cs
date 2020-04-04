using System;
using System.Runtime.InteropServices;
using System.Text;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Adaptors;
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

        private static InstallWatchTx install_watch_tx_ptr =
            (ref FFISha256dHash txid,  ref FFIScript scriptPubKey) =>
            {
                Console.WriteLine($"Installing watch tx with txid ({Hex.Encode(txid.AsSpan())}) scriptPubKey {Hex.Encode(scriptPubKey.AsSpan())}");
            };

        private static InstallWatchOutPoint install_watch_outpoint =
            (ref FFIOutPoint outpoint, ref FFIScript script) =>
            {
                Console.WriteLine($"Installing watch outpoint with {Hex.Encode(outpoint.script_pub_key.AsSpan())}, spk {Hex.Encode(script.AsSpan())}");
            };

        private static WatchAllTxn watch_all_txn =
            () =>
            {
                Console.WriteLine("watch all txn");
            };

        private static GetChainUtxo get_chain_utxo =
            (ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError err,
                ref FFITxOut txOut) =>
            {
                Console.WriteLine($"get_chain_utxo {Hex.Encode(genesisHash.AsSpan())}");
            };

        private static FilterBlock filter_block =
            (ref FFIBlock block) =>
            {
                Console.WriteLine($"filtering block {Hex.Encode(block.AsSpan())}");
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
        
        public static ChannelMonitor Create()
        {
            Interop.create_chain_watch_interface(ref install_watch_tx_ptr, ref install_watch_outpoint, ref watch_all_txn, ref get_chain_utxo, ref filter_block, out var chainWatchInterfaceHandle);
            Interop.create_broadcaster(ref broadcast_ptr, out var broadcasterHandle);
            Interop.create_logger(ref logConsole, out var loggerHandle);
            Interop.create_fee_estimator(ref get_est_sat_per_1000_weight, out var feeEstimatorHandle);
            
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