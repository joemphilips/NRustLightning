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
        
        public 
        public InstallWatchTx InstallWatchTx => this.InstallWatchTxCore;
        public InstallWatchOutPoint InstallWatchOutPoint { get; }
        public WatchAllTxn WatchAllTxn { get; }
        public GetChainUtxo GetChainUtxo { get; }
        public FilterBlock FilterBlock { get; }
    }
}