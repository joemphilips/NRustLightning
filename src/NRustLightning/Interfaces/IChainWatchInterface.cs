using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    /// <summary>
    /// User defined interface for watching blockchain.
    /// Important: It has to hold delegates as a static field.
    /// Otherwise the delegate passed to the rust side becomes null pointer and crashes.
    ///
    /// keep in mind that the static delegate defined in this class may call the
    /// delegate from multiple threads at the same time, so it must make thread safe,
    /// If you want to hold mutable state in it.
    /// </summary>
    public interface IChainWatchInterface
    {
        ref InstallWatchTx InstallWatchTx { get; }
        ref InstallWatchOutPoint InstallWatchOutPoint { get; }
        ref WatchAllTxn WatchAllTxn { get; }
        ref GetChainUtxo GetChainUtxo { get; }
        ref FilterBlock FilterBlock { get; }
    }
}