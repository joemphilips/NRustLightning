using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Interfaces;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Tests.Stubs
{
    public class TestPeerManagerProvider : IPeerManagerProvider
    {
        private readonly IBroadcaster _broadcaster;
        private readonly IFeeEstimator _feeEstimator;
        private readonly IChainWatchInterface _chainWatchInterface;
        private readonly PeerManager _peerManager;

        public TestPeerManagerProvider(IBroadcaster broadcaster, IFeeEstimator feeEstimator, IChainWatchInterface chainWatchInterface, IOptions<Config> config, ILoggerFactory loggerFactory, IKeysRepository keysRepository)
        {
            _broadcaster = broadcaster;
            _feeEstimator = feeEstimator;
            _chainWatchInterface = chainWatchInterface;
            var seed = new byte[32];
            var n = NRustLightning.Adaptors.Network.RegTest;
            var conf = config.Value.RustLightningConfig;
            var logger = new NativeLogger(loggerFactory.CreateLogger<NativeLogger>());
            _peerManager = 
                PeerManager.Create(seed.AsSpan(), in n, in conf, chainWatchInterface, broadcaster, logger, feeEstimator, 100, keysRepository.GetNodeSecret().ToBytes());
        }
        
        public PeerManager? GetPeerManager(string cryptoCode)
        {
            Debug.Assert(cryptoCode.ToLowerInvariant() == "btc");
            return _peerManager;
        }

        public PeerManager? GetPeerManager(NRustLightningNetwork network)
        {
            Debug.Assert(network.NBitcoinNetwork.NetworkType == NetworkType.Regtest);
            return GetPeerManager(network.CryptoCode);
        }
    }
}