using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NBXplorer;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Server.Services;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.FFIProxies
{
    public class NBXChainWatchInterface : IChainWatchInterface
    {
        private readonly ILogger<NBXChainWatchInterface> logger;
        private readonly NRustLightningNetwork network;
        private InstallWatchTx installWatchTx;
        private InstallWatchOutPoint installWatchOutPoint;
        private WatchAllTxn watchAllTxn;
        private GetChainUtxo getChainUtxo;
        
        /// <summary>
        /// This will be incremented every time new tx or outpoint
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
        // ---

        public NBXChainWatchInterface(ExplorerClient nbxplorerClient, ILogger<NBXChainWatchInterface> logger, NRustLightningNetwork network)
        {
            this.logger = logger;
            this.network = network;
            NbxplorerClient = nbxplorerClient;
            installWatchTx = (ref FFISha256dHash hash, ref FFIScript script) =>
            {
                if (isWatchAll) return;
                if (watchedTxs.TryAdd(script.ToScript(), 0))
                {
                    Interlocked.Increment(ref reEnteredCount);
                }
            };
            installWatchOutPoint = (ref FFIOutPoint outPoint, ref FFIScript script) =>
            {
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
            getChainUtxo = (ref FFISha256dHash hash, ulong id, ref ChainError error, ref FFITxOut txout) =>
            {
                if (hash.ToUInt256() != network.NBitcoinNetwork.GenesisHash)
                {
                    error = ChainError.NotWatched;
                }
                else
                {
                    error = ChainError.NotSupported;
                }
            };
        }
        public ExplorerClient NbxplorerClient { get; }
        
        public ref InstallWatchTx InstallWatchTx => ref installWatchTx;

        public ref InstallWatchOutPoint InstallWatchOutPoint => ref installWatchOutPoint;

        public ref WatchAllTxn WatchAllTxn => ref watchAllTxn;

        public ref GetChainUtxo GetChainUtxo => ref getChainUtxo;

    }
}