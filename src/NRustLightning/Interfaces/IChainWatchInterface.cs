using System;
using System.Collections.Generic;
using NBitcoin;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    /// <summary>
    /// User defined interface for watching blockchain.
    ///
    /// keep in mind that the delegate defined in this class may call the
    /// delegate from multiple threads at the same time, so it must make thread safe
    /// if you want to hold mutable state in it.
    /// </summary>
    public interface IChainWatchInterface
    {
        /// <summary>
        /// Provides a txid/random-scriptPubKey-in-the-tx which must be watched for.
        /// </summary>
        InstallWatchTx InstallWatchTx { get; }
        /// <summary>
        /// Provides an outpoint which must be watched for, providing any transactions which spend the given outpoint.
        /// </summary>
        InstallWatchOutPoint InstallWatchOutPoint { get; }
        /// <summary>
        /// Indicates that a listener needs to see all transactions.
        /// </summary>
        WatchAllTxn WatchAllTxn { get; }
        /// <summary>
        /// Gets the script and value in satoshis for a given unspent transaction output given a short_channel_id
        /// (a.k.a. unspent_tx_output_identifier). For BTC/tBTC channels the top three
        /// bytes are the block height, the next 3 the transaction index within the block and the
        /// final two the output within the tx.
        /// </summary>
        GetChainUtxo GetChainUtxo { get; }
        
        FilterBlock FilterBlock { get; }
    }

    public abstract class ChainWatchInterface : IChainWatchInterface
    {

        protected abstract void InstallWatchTxImpl(uint256 txid, Script spk);
        protected abstract void InstallWatchOutPointImpl(OutPoint outpoint, Script spk);

        protected abstract bool TryGetChainUtxoImpl(uint256 genesisBlockHash, ulong utxoId, out ChainError error, out Script scriptPubKey, out Money amount);

        protected abstract (IList<Transaction>, IList<uint>) FilterBlockImpl(Block b);

        protected virtual void InstallWatchTxCore(ref FFISha256dHash txid, ref FFIScript spk)
        {
            InstallWatchTxImpl(txid.ToUInt256(), spk.ToScript());
        }

        protected virtual void InstallWatchOutPointCore(ref FFIOutPoint ffiOutPoint, ref FFIScript outScript)
        {
            var t = ffiOutPoint.ToTuple();
            var outpoint = new OutPoint(t.Item1, t.Item2);
            InstallWatchOutPointImpl(outpoint, outScript.ToScript());
        }
        
        protected void WatchAll() {}

        protected virtual void GetChainUtxoCore(ref FFISha256dHash genesisHash, ulong utxoId, ref ChainError error,
            ref byte scriptPtr, ref UIntPtr scriptLen, ref ulong amountSatoshis)
        {
        }

        protected virtual void FilterBlockCore(ref byte blockPtr, UIntPtr blockLen, ref byte matchedTxPtr, ref UIntPtr matchedTxLen, ref byte matchedIndexPtr, ref UIntPtr matchedIndexLen)
        {
            throw new NotImplementedException();
        }
        
        public InstallWatchTx InstallWatchTx => this.InstallWatchTxCore;
        public InstallWatchOutPoint InstallWatchOutPoint => InstallWatchOutPointCore;
        public WatchAllTxn WatchAllTxn => WatchAll;
        public GetChainUtxo GetChainUtxo => GetChainUtxoCore;
        public FilterBlock FilterBlock => FilterBlockCore;
    }
}