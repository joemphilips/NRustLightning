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
using NRustLightning.Utils;

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
            _chainWatchInterface= chainWatchInterface;
            var seed = new byte[32];
            var n= NBitcoin.Network.RegTest;
            var conf = config.Value.RustLightningConfig;
            var logger = new NativeLogger(loggerFactory.CreateLogger<NativeLogger>());
            var keysInterface = new KeysManager(seed, DateTime.UnixEpoch);
            _peerManager = 
                PeerManager.Create(seed.AsSpan(), n, conf, chainWatchInterface, keysInterface,  broadcaster, logger, feeEstimator, 100);
        }
        
        public PeerManager? TryGetPeerManager(string cryptoCode)
        {
            Debug.Assert(cryptoCode.ToLowerInvariant() == "btc");
            return _peerManager;
        }

        public PeerManager? TryGetPeerManager(NRustLightningNetwork network)
        {
            Debug.Assert(network.NBitcoinNetwork.NetworkType == NetworkType.Regtest);
            return TryGetPeerManager(network.CryptoCode);
        }
    }
}