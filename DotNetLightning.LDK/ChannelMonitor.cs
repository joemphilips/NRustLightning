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
        private readonly ChannelMonitorHandle _handle;
        internal ChannelMonitor(ChannelMonitorHandle handle)
        {
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
            (ref FFIChainWatchInterface self, ref FFISha256dHash txid,  ref FFIScript scriptPubKey) =>
            {
                Console.WriteLine($"Installing watch tx with txid ({Hex.Encode(txid.AsSpan())}) scriptPubKey {Hex.Encode(scriptPubKey.AsSpan())}");
            };

        private static InstallWatchOutPoint install_watch_outpoint =
            (ref FFIChainWatchInterface self, ref FFIOutPoint outpoint, ref FFIScript script) =>
            {
                Console.WriteLine($"Installing watch outpoint with {Hex.Encode(outpoint.script_pub_key.AsSpan())}, spk {Hex.Encode(script.AsSpan())}");
            };

        private static WatchAllTxn watch_all_txn =
            (ref FFIChainWatchInterface self) =>
            {
                Console.WriteLine("watch all txn");
            };

        private static GetChainUtxo get_chain_utxo =
            (ref FFIChainWatchInterface self, ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError err,
                ref FFITxOut txOut) =>
            {
                Console.WriteLine($"get_chain_utxo {Hex.Encode(genesisHash.AsSpan())}");
            };

        private static FilterBlock filter_block =
            (ref FFIChainWatchInterface self, ref FFIBlock block) =>
            {
                Console.WriteLine($"filtering block {Hex.Encode(block.AsSpan())}");
            };

        private static Log logConsole = (ref FFILogger self, ref FFILogRecord record) =>
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
            // we must make a copy of these delegates here.
            var chainwatcher = new FFIChainWatchInterface();
            chainwatcher.InstallWatchTx = install_watch_tx_ptr;
            chainwatcher.InstallWatchOutPoint = install_watch_outpoint;
            chainwatcher.WatchAllTxn = watch_all_txn;
            chainwatcher.GetChainUtxo = get_chain_utxo;
            chainwatcher.FilterBlock = filter_block;
            
            var broadcaster = new FFIBroadcaster();
            
            var logger = new FFILogger();
            logger.log = logConsole;
            
            var feeEstimator = new FFIFeeEstimator();
            feeEstimator.get_est_sat_per_1000_weight = get_est_sat_per_1000_weight;
            
            Interop.create_ffi_channel_monitor(ref chainwatcher, ref broadcaster, ref logger, ref feeEstimator, out var handle);
            return new ChannelMonitor(handle);
        }
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}