using System.Collections.Generic;
using System.Linq;
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

        public ExplorerClient? TryGetClient(string cryptoCode)
        {
            return GetClient(cryptoCode);
        }

        public IEnumerable<ExplorerClient> GetAll()
        {
            return _networkProvider.GetAll().Select(n => new ExplorerClient(n.NbXplorerNetwork));
        }
    }
}