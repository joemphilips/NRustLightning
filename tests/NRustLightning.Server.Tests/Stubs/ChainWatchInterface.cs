using System;
using NRustLightning.Handles;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Server.Tests.Stubs
            
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
            };
            _watchAllTxn =
            () =>
            {
                Console.WriteLine("watch all txn");
            };

            _getChainUtxo = (ref FFISha256dHash genesisHash, ulong id, ref ChainError error, ref byte scriptPtr, ref UIntPtr scriptLen, ref ulong amountSatoshi) =>
                {
                    error = ChainError.NotWatched;
                };
        }
        public InstallWatchTx InstallWatchTx => _installWatchTx;

        public InstallWatchOutPoint InstallWatchOutPoint => _installWatchOutpoint;

        public WatchAllTxn WatchAllTxn => _watchAllTxn;

        public GetChainUtxo GetChainUtxo => _getChainUtxo;
        public FilterBlock FilterBlock => _filterBlock;
    }
}