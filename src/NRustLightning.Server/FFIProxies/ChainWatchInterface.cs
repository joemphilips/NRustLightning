using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using DotNetLightning.Utils;
using NBXplorer;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Server.Services;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.Server.Networks;
using NRustLightning.Utils;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.FFIProxies
{
    public class NbxChainWatchInterface : IChainWatchInterface
    {
        private readonly ILogger<NbxChainWatchInterface> _logger;
        private readonly NRustLightningNetwork _network;
        
        private IChainWatchInterface util;

        public NbxChainWatchInterface(ExplorerClient nbxplorerClient, ILogger<NbxChainWatchInterface> logger, NRustLightningNetwork network)
        {
            _logger = logger;
            _network = network;
            NbxplorerClient = nbxplorerClient;
            util = new ChainWatchInterfaceUtil(network.NBitcoinNetwork);
        }
        
        public ExplorerClient NbxplorerClient { get; }
        
        public Network Network => _network.NBitcoinNetwork;
        public void InstallWatchTxImpl(uint256 txid, Script spk)
        {
            _logger.LogDebug($"watching new tx of id: {txid}. output scriptPubKey is {spk.ToHex()}");
            util.InstallWatchTxImpl(txid, spk);
        }

        public void InstallWatchOutPointImpl(OutPoint outpoint, Script spk)
        {
            _logger.LogDebug($"watching new outpoint (prevHash: {outpoint.Hash}, prevOutIndex: {outpoint.N})");
            util.InstallWatchOutPointImpl(outpoint, spk);
        }

        public bool TryGetChainUtxoImpl(uint256 genesisBlockHash, ulong utxoId, ref ChainError error, out Script script,
            out Money amountSatoshi)
        {
            script = null;
            amountSatoshi = null;
            var shortChannelId = Primitives.ShortChannelId.FromUInt64(utxoId);
            var b = NbxplorerClient.RPCClient.GetBlock(shortChannelId.BlockHeight.Item);
            if (b.Transactions.Count > shortChannelId.BlockIndex.Item)
            {
                error = ChainError.UnknownTx;
                return false;
            }
            var tx = b.Transactions[(int)shortChannelId.BlockIndex.Item];
            if (tx.Outputs.Count > shortChannelId.TxOutIndex.Item)
            {
                error = ChainError.UnknownTx;
                return false;
            }

            var txOut = tx.Outputs[shortChannelId.TxOutIndex.Item];

            script = txOut.ScriptPubKey;
            amountSatoshi = (ulong)txOut.Value.Satoshi;
            return true;
        }

        public void WatchAllTxnImpl()
        {
            util.WatchAllTxnImpl();
        }

        public List<uint> FilterBlockImpl(Block b)
        {
            return util.FilterBlockImpl(b);
        }

        public int ReEntered()
        {
            return util.ReEntered();
        }
    }
}