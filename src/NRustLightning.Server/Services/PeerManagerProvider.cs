using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBXplorer;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Repository;

namespace NRustLightning.Server.Services
{
    public class PeerManagerProvider : IPeerManagerProvider, IHostedService
    {
        private readonly INBXplorerClientProvider _nbXplorerClientProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly IKeysRepository _keysRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ChannelProvider _channelProvider;
        private readonly IOptions<Config> _config;
        private readonly RepositoryProvider _repositoryProvider;
        private Dictionary<string, PeerManager> _peerManagers = new Dictionary<string, PeerManager>();

        public PeerManagerProvider(
            INBXplorerClientProvider nbXplorerClientProvider,
            NRustLightningNetworkProvider networkProvider,
            IKeysRepository keysRepository,
            ILoggerFactory loggerFactory,
            ChannelProvider channelProvider,
            IOptions<Config> config,
            RepositoryProvider repositoryProvider
            )
        {
            _nbXplorerClientProvider = nbXplorerClientProvider;
            _networkProvider = networkProvider;
            _keysRepository = keysRepository;
            _loggerFactory = loggerFactory;
            _channelProvider = channelProvider;
            _config = config;
            _repositoryProvider = repositoryProvider;
        }

        public PeerManager? TryGetPeerManager(string cryptoCode)
        {
            _peerManagers.TryGetValue(cryptoCode.ToLowerInvariant(), out var p);
            return p;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var n in _networkProvider.GetAll())
            {
                var nbx = _nbXplorerClientProvider.TryGetClient(n);
                if (nbx != null)
                {
                    var b = new NbXplorerBroadcaster(nbx, _loggerFactory.CreateLogger<NbXplorerBroadcaster>());
                    var feeEst = new NbXplorerFeeEstimator(_loggerFactory.CreateLogger<NbXplorerFeeEstimator>(),
                        _channelProvider.GetFeeRateChannel(n).Reader);
                    var chainWatchInterface =
                        new NbxChainWatchInterface(nbx, _loggerFactory.CreateLogger<NbxChainWatchInterface>(), n);
                    var conf = _config.Value.RustLightningConfig;
                    var peerManSeed = new byte[32];
                    RandomUtils.GetBytes(peerManSeed);
                    
                    var logger = new NativeLogger(_loggerFactory.CreateLogger<NativeLogger>());

                    /// TODO
                    uint currentBlockHeight = 0;
                    
                    var maybeChanMan = await _repositoryProvider.GetRepository(n).GetChannelManager(new ChannelManagerReadArgs(_keysRepository, b, feeEst, logger), cancellationToken);
                    PeerManager peerMan;
                    if (maybeChanMan is null)
                    {
                        peerMan = PeerManager.Create(peerManSeed.AsSpan(), n.NBitcoinNetwork, conf, chainWatchInterface,
                            _keysRepository, b,
                            logger, feeEst, currentBlockHeight);
                    }
                    else
                    {
                        var c = conf.GetUserConfig();
                        peerMan = PeerManager.Create(peerManSeed.AsSpan(), n.NBitcoinNetwork, in c, chainWatchInterface, logger, _keysRepository.GetNodeSecret().ToBytes(), maybeChanMan, currentBlockHeight);
                    }
                    _peerManagers.Add(n.CryptoCode, peerMan);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

            foreach (var n in _networkProvider.GetAll())
            {
                var repo = _repositoryProvider.GetRepository(n);
                if (repo != null)
                {
                    await repo.SetChannelManager(_peerManagers[n.CryptoCode].ChannelManager, cancellationToken);
                }
            }
        }
    }
}