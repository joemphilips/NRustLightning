using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using NBitcoin;

namespace NRustLightning.Utils
{
    /// <summary>
    /// This is a port of the class with the same name in rust-lightning.
    /// Useful for implementing ChainWatchInterface
    /// </summary>
    public class ChainWatchInterfaceUtil
    {
        private readonly Network _network;
        private ChainWatchedUtil watched = new ChainWatchedUtil();
        private int _reentered = 1;

        public ChainWatchInterfaceUtil(Network network)
        {
            _network = network ?? throw new ArgumentNullException(nameof(network));
        }

        public void InstallWatchTx(uint256 txid, Script scriptPubKey)
        {
            if (watched.RegisterTx(txid, scriptPubKey))
            {
                Interlocked.Increment(ref _reentered);
            }
        }

        public void InstallWatchOutpoint(OutPoint outpoint, Script outScript)
        {
            if (watched.RegisterOutPoint(outpoint))
            {
                Interlocked.Increment(ref _reentered);
            }
        }

        public void WatchAllTxn()
        {
            if (watched.WatchAll())
            {
                Interlocked.Increment(ref _reentered);
            }
        }


        public (List<Transaction>, List<uint>) FilterBlock(Block block)
        {
            var matched = new List<Transaction>();
            var matchedIndex = new List<uint>();
            foreach (var (tx, i) in block.Transactions.Select((tx, i) => (tx, i)))
            {
                if (watched.DoesMatchTx(tx))
                {
                    matched.Add(tx);
                    matchedIndex.Add((uint) i);
                }
            }

            return (matched, matchedIndex);
        }

        public UIntPtr Reentered {
            get => (UIntPtr) _reentered;
        }
    }

    internal class ChainWatchedUtil
    {
        private int _watchAll = 0;

        internal bool watchAll
        {
            get => _watchAll == 1;
            set => Interlocked.Exchange(ref _watchAll, (value ? 1 : 0));
        }
        private object watchedTxnLock = new object();
        private object watchedOutPointsLock = new object();
        internal HashSet<Script> watchedTxn = new HashSet<Script>();
        internal HashSet<OutPoint> watchedOutPoints = new HashSet<OutPoint>();

        public bool RegisterTx(uint256 txId, Script scriptPubKey)
        {
            if (watchAll) { return false; }
            lock (watchedTxnLock)
            {
                return this.watchedTxn.Add(scriptPubKey);
            }
        }

        public bool RegisterOutPoint(OutPoint outPoint)
        {
            if (watchAll)
            {
                return false; 
            }

            lock (watchedOutPointsLock)
            {
                return this.watchedOutPoints.Add(outPoint);
            }
        }

        public bool WatchAll()
        {
            if (watchAll)
            {
                return false;
            }
            watchAll = true;
            return true;
        }

        public bool DoesMatchTx(Transaction tx)
        {
            if (watchAll)
            {
                return true;
            }

            foreach (var o in tx.Outputs)
            {
                lock (watchedTxnLock)
                {
                    foreach (var s in watchedTxn)
                    {
                        if (s == o.ScriptPubKey)
                            return true;
                    }
                }
            }

            foreach (var i in tx.Inputs)
            {
                lock (watchedOutPointsLock)
                {
                    foreach (var outpoint in watchedOutPoints)
                    {
                        if (outpoint.Hash == i.PrevOut.Hash && outpoint.N == i.PrevOut.N)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}