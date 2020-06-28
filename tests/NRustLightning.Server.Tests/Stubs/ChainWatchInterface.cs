using System;
using System.Collections.Generic;
using NBitcoin;
using NRustLightning.Handles;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Utils;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.Tests.Stubs
            
{
    public class TestChainWatchInterface : IChainWatchInterface
    {

        public TestChainWatchInterface(Network n)
        {
            Network = n ?? throw new ArgumentNullException(nameof(n));
        }

        public Network Network { get; }

        public void InstallWatchTxImpl(uint256 txid, Script spk)
        {
            Console.WriteLine($"Installing watch tx with txid ({txid}) scriptPubKey {spk}");
        }

        public void InstallWatchOutPointImpl(OutPoint outpoint, Script spk)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChainUtxoImpl(uint256 genesisBlockHash, ulong utxoId, ref ChainError error, out Script scriptPubKey,
            out Money amount)
        {
            error = ChainError.NotWatched;
            scriptPubKey = null;
            amount = null;
            return false;
        }

        public void WatchAllTxnImpl()
        {
            Console.WriteLine("watch all txn");
        }

        public List<uint> FilterBlockImpl(Block b)
        {
            throw new NotImplementedException();
        }

        public bool ReEntered()
        {
            throw new NotImplementedException();
        }
    }
}