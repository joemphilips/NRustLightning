using NBXplorer;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Tests.Stubs
{
    public class StubNBXplorerClientProvider : INBXplorerClientProvider
    {
        private readonly NRustLightningNetworkProvider _networkProvider;
        public StubNBXplorerClientProvider(NRustLightningNetworkProvider networkProvider)
        {
            _networkProvider = networkProvider;
        }
        public ExplorerClient GetClient(string cryptoCode)
        {
            return new ExplorerClient(_networkProvider.GetByCryptoCode(cryptoCode).NbXplorerNetwork);
        }
    }
}