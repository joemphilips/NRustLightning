using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Services
{
    public class PeerManagerProvider : IPeerManagerProvider
    {
        private Dictionary<string, PeerManager> _peerManagers = new Dictionary<string, PeerManager>();

        public PeerManagerProvider(
            INBXplorerClientProvider nbXplorerClientProvider,
            NRustLightningNetworkProvider networkProvider,
            IKeysRepository keysRepository,
            ILoggerFactory loggerFactory,
            ChannelProvider channelProvider,
            IOptions<Config> config
            )
        {
            foreach (var n in networkProvider.GetAll())
            {
                var nbx = nbXplorerClientProvider.TryGetClient(n);
                if (nbx != null)
                {
                    var b = new NbXplorerBroadcaster(nbx, loggerFactory.CreateLogger<NbXplorerBroadcaster>());
                    var feeEst = new NbXplorerFeeEstimator(loggerFactory.CreateLogger<NbXplorerFeeEstimator>(),
                        channelProvider.GetFeeRateChannel(n).Reader);
                    var chainWatchInterface =
                        new NbxChainWatchInterface(nbx, loggerFactory.CreateLogger<NbxChainWatchInterface>(), n);
                    var seed = new byte[32];
                    RandomUtils.GetBytes(seed);
                    var keysInterface = keysRepository.GetKeysInterface(seed);
                    var logger = new NativeLogger(loggerFactory.CreateLogger<NativeLogger>());
                    var nbitcoinNetwork = n.NBitcoinNetwork;
                    var conf = config.Value.RustLightningConfig;
                    var peerMan = PeerManager.Create(seed.AsSpan(), nbitcoinNetwork, conf, chainWatchInterface, keysInterface ,b,
                        logger, feeEst, 400000);
                    _peerManagers.Add(n.CryptoCode, peerMan);
                }
            }
        }

        public PeerManager? TryGetPeerManager(string cryptoCode)
        {
            _peerManagers.TryGetValue(cryptoCode.ToLowerInvariant(), out var p);
            return p;
        }
    }
}