using System;
using NRustLightning.Handles;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Tests.Utils
{
    public class TestChainWatchInterface : IChainWatchInterface
    {

        private InstallWatchTx _installWatchTx;
        private InstallWatchOutPoint _installWatchOutpoint;

        private WatchAllTxn _watchAllTxn;
        private GetChainUtxo _getChainUtxo;
        private FilterBlock _filterBlock;
        public TestChainWatchInterface()
        {
            _installWatchTx =
            (ref FFISha256dHash txid,  ref FFIScript scriptPubKey) =>
            {
                Console.WriteLine($"Installing watch tx with txid ({Hex.Encode(txid.AsSpan())}) scriptPubKey {Hex.Encode(scriptPubKey.AsSpan())}");
            };

            _installWatchOutpoint =
            (ref FFIOutPoint outpoint, ref FFIScript script) =>
            {
                Console.WriteLine($"Installing watch outpoint with {Hex.Encode(outpoint.script_pub_key.AsSpan())}, spk {Hex.Encode(script.AsSpan())}");
            };
            _watchAllTxn =
            () =>
            {
                Console.WriteLine("watch all txn");
            };

            _getChainUtxo = 
            (ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError err,
                ref FFITxOut txOut) =>
            {
                Console.WriteLine($"get_chain_utxo {Hex.Encode(genesisHash.AsSpan())}");
            };
            _filterBlock =
            (ref FFIBlock block) =>
            {
                Console.WriteLine($"filtering block {Hex.Encode(block.AsSpan())}");
            };


        }
        public ref InstallWatchTx InstallWatchTx => ref _installWatchTx;

        public ref InstallWatchOutPoint InstallWatchOutPoint => ref _installWatchOutpoint;

        public ref WatchAllTxn WatchAllTxn => ref _watchAllTxn;

        public ref GetChainUtxo GetChainUtxo => ref _getChainUtxo;

        public ref FilterBlock FilterBlock => ref _filterBlock;
    }
}