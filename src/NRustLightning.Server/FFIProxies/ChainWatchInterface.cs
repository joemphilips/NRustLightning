using NBXplorer;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.FFIProxies
{
    public class NBXChainWatchInterface : IChainWatchInterface
    {
        private InstallWatchTx installWatchTx;
        private InstallWatchOutPoint installWatchOutPoint;
        private WatchAllTxn watchAllTxn;
        private GetChainUtxo getChainUtxo;
        private FilterBlock filterBlock;

        public NBXChainWatchInterface(ExplorerClient nbxplorerClient)
        {
            NbxplorerClient = nbxplorerClient;
        }
        public ExplorerClient NbxplorerClient { get; }
        
        public ref InstallWatchTx InstallWatchTx => ref installWatchTx;

        public ref InstallWatchOutPoint InstallWatchOutPoint => ref installWatchOutPoint;

        public ref WatchAllTxn WatchAllTxn => ref watchAllTxn;

        public ref GetChainUtxo GetChainUtxo => ref getChainUtxo;

        public ref FilterBlock FilterBlock => ref filterBlock;
    }
}