using System;
using System.Collections.Generic;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Interfaces;
using DotNetLightning.LDK.Utils;
namespace DotNetLightning.LDK.Tests
{
    internal class TestBroadcaster : IBroadcaster
    {
        internal static List<string> BroadcastedTxHex { get; } = new List<string>();
        private static FFIBroadcastTransaction _broadcast = (ref FFITransaction tx) =>
        {
            var hex = Hex.Encode(tx.AsSpan());
            BroadcastedTxHex.Add(hex);
        };
        ref FFIBroadcastTransaction IBroadcaster.BroadcastTransaction
            => ref _broadcast;
    }

    internal class TestFeeEstimator : IFeeEstimator
    {
        private static FFIGetEstSatPer1000Weight _estimate = (ref FFITransaction tx) => 1000;
        public ref FFIGetEstSatPer1000Weight getEstSatPer1000Weight => ref _estimate;
    }
    internal class TestLogger : ILogger
    {
        private Log _log = (ref FFILogRecord record) =>
        {
            Console.WriteLine(record.args);
        };

        public ref Log Log => ref _log;
    }

    internal class TestChainWatchInterface : IChainWatchInterface
    {
        private static InstallWatchTx _installWatchTx =
            (ref FFISha256dHash txid,  ref FFIScript scriptPubKey) =>
            {
                Console.WriteLine($"Installing watch tx with txid ({Hex.Encode(txid.AsSpan())}) scriptPubKey {Hex.Encode(scriptPubKey.AsSpan())}");
            };

        private static InstallWatchOutPoint _installWatchOutpoint =
            (ref FFIOutPoint outpoint, ref FFIScript script) =>
            {
                Console.WriteLine($"Installing watch outpoint with {Hex.Encode(outpoint.script_pub_key.AsSpan())}, spk {Hex.Encode(script.AsSpan())}");
            };

        private static WatchAllTxn _watchAllTxn =
            () =>
            {
                Console.WriteLine("watch all txn");
            };

        private static GetChainUtxo _getChainUtxo =
            (ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError err,
                ref FFITxOut txOut) =>
            {
                Console.WriteLine($"get_chain_utxo {Hex.Encode(genesisHash.AsSpan())}");
            };

        private static FilterBlock _filterBlock =
            (ref FFIBlock block) =>
            {
                Console.WriteLine($"filtering block {Hex.Encode(block.AsSpan())}");
            };

        public ref InstallWatchTx InstallWatchTx => ref _installWatchTx;

        public ref InstallWatchOutPoint InstallWatchOutPoint => ref _installWatchOutpoint;

        public ref WatchAllTxn WatchAllTxn => ref _watchAllTxn;

        public ref GetChainUtxo GetChainUtxo => ref _getChainUtxo;

        public ref FilterBlock FilterBlock => ref _filterBlock;
    }
}