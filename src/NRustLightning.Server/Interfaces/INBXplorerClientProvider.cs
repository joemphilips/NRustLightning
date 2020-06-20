using System.Collections.Generic;
using NBXplorer;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface INBXplorerClientProvider
    {
        public ExplorerClient GetClient(string cryptoCode);
        public ExplorerClient GetClient(NRustLightningNetwork n) => GetClient(n.CryptoCode);

        public ExplorerClient? TryGetClient(string cryptoCode);
        public ExplorerClient? TryGetClient(NRustLightningNetwork n) => TryGetClient(n.CryptoCode);

        public IEnumerable<ExplorerClient> GetAll();
    }
}