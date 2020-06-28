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
using Network = NBitcoin.Network;

namespace NRustLightning.Server.FFIProxies
{
    public class NbxChainWatchInterface : IChainWatchInterface
    {
        private readonly ILogger<NbxChainWatchInterface> logger;
        private readonly NRustLightningNetwork network;
        private InstallWatchTx installWatchTx;
        private InstallWatchOutPoint installWatchOutPoint;
        private WatchAllTxn watchAllTxn;
        private GetChainUtxo getChainUtxo;
        private FilterBlock filterBlock;
        
        /// <summary>
        /// This will be incremented every time new tx or outpoint comes in.
        /// </summary>
        private long reEnteredCount = 1;
        
        // --- ChainWatchedUtil members ---
        private bool isWatchAll = false;
        /// <summary>
        /// There is no `ConcurrentHashSet` in std lib. So we use dict with meaningless value.
        /// </summary>
        private ConcurrentDictionary<Script, byte> watchedTxs = new ConcurrentDictionary<Script, byte>();
        /// <summary>
        /// There is no `ConcurrentHashSet` in std lib. So we use dict with meaningless value.
        /// </summary>
        private ConcurrentDictionary<(uint256, uint), byte> watchedOutpoints = new ConcurrentDictionary<(uint256, uint), byte>();

        public NbxChainWatchInterface(ExplorerClient nbxplorerClient, ILogger<NbxChainWatchInterface> logger, NRustLightningNetwork network)
        {
            this.logger = logger;
            this.network = network;
            NbxplorerClient = nbxplorerClient;
            installWatchTx = (ref FFISha256dHash hash, ref FFIScript script) =>
            {
                if (isWatchAll) return;
                logger.LogDebug($"watching new tx {hash.AsArray().ToHexString()}");
                if (watchedTxs.TryAdd(script.ToScript(), 0))
                {
                    Interlocked.Increment(ref reEnteredCount);
                }
            };
            installWatchOutPoint = (ref FFIOutPoint outPoint, ref FFIScript script) =>
            {
                var n = outPoint.ToTuple().Item2;
                logger.LogDebug($"watching new outpoint {outPoint.GetTxId()}. index: {n}");
                if (isWatchAll) return;
                if (watchedOutpoints.TryAdd(outPoint.ToTuple(), 0))
                {
                    Interlocked.Increment(ref reEnteredCount);
                }
            };
            watchAllTxn = () =>
            {
                if (!isWatchAll)
                {
                    isWatchAll = true;
                    Interlocked.Increment(ref reEnteredCount);
                }
            };
            getChainUtxo = (ref FFISha256dHash chainGenesis, ulong id, ref ChainError error, ref byte scriptPtr, ref UIntPtr scriptLen, ref ulong amountSatoshi) =>
            {
                var shortChannelId = Primitives.ShortChannelId.FromUInt64(id);
                var b = nbxplorerClient.RPCClient.GetBlock(shortChannelId.BlockHeight.Item);
                if (b.Transactions.Count > shortChannelId.BlockIndex.Item)
                {
                    error = ChainError.UnknownTx;
                    return;
                }
                var tx = b.Transactions[(int)shortChannelId.BlockIndex.Item];
                if (tx.Outputs.Count > shortChannelId.TxOutIndex.Item)
                {
                    error = ChainError.UnknownTx;
                    return;
                }

                var txOut = tx.Outputs[shortChannelId.TxOutIndex.Item];
                var scriptPubKeyBytes = txOut.ScriptPubKey.ToBytes();
                scriptLen = (UIntPtr)scriptPubKeyBytes.Length;
                // copy into unmanaged memory.
                Unsafe.CopyBlock(ref scriptPtr, ref scriptPubKeyBytes[0], (uint)scriptPubKeyBytes.Length);
                amountSatoshi = (ulong)txOut.Value.Satoshi;
            };
        }
        
        public ExplorerClient NbxplorerClient { get; }
        
        public InstallWatchTx InstallWatchTx => installWatchTx;

        public InstallWatchOutPoint InstallWatchOutPoint => installWatchOutPoint;

        public WatchAllTxn WatchAllTxn => watchAllTxn;

        public GetChainUtxo GetChainUtxo => getChainUtxo;

        public FilterBlock FilterBlock => filterBlock;
        public Network Network { get; }
        public void InstallWatchTxImpl(uint256 txid, Script spk)
        {
            throw new NotImplementedException();
        }

        public void InstallWatchOutPointImpl(OutPoint outpoint, Script spk)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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