using NBXplorer;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface INBXplorerClientProvider
    {
        public ExplorerClient GetClient(string cryptoCode);
        public ExplorerClient GetClient(NRustLightningNetwork n) => GetClient(n.CryptoCode);
        
    }
}