using System;
using System.Collections.Generic;
using NBXplorer;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface INBXplorerClientProvider
    {
        public ExplorerClient GetClient(string cryptoCode);

        public ExplorerClient GetClient(NRustLightningNetwork n)
        {
            if (n == null) throw new ArgumentNullException(nameof(n));
            return GetClient(n.CryptoCode.ToLowerInvariant());
        }

        public ExplorerClient? TryGetClient(string cryptoCode);

        public ExplorerClient? TryGetClient(NRustLightningNetwork n)
        {
            if (n == null) throw new ArgumentNullException(nameof(n));
            return TryGetClient(n.CryptoCode.ToLowerInvariant());
        }

        public IEnumerable<ExplorerClient> GetAll();
    }
}