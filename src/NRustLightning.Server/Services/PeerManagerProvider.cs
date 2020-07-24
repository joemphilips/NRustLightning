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
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server.Services
{
    public class PeerManagerProvider : IHostedService
    {
        private readonly INBXplorerClientProvider _nbXplorerClientProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly IKeysRepository _keysRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PeerManagerProvider> _logger;
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
            _logger = loggerFactory.CreateLogger<PeerManagerProvider>();
            _channelProvider = channelProvider;
            _config = config;
            _repositoryProvider = repositoryProvider;
        }

        public PeerManager? TryGetPeerManager(string cryptoCode)
        {
            _peerManagers.TryGetValue(cryptoCode.ToLowerInvariant(), out var p);
            return p;
        }

        public PeerManager? TryGetPeerManager(NRustLightningNetwork n) => TryGetPeerManager(n.CryptoCode);

        public PeerManager GetPeerManager(string cryptoCode) =>
            TryGetPeerManager(cryptoCode) ?? NRustLightning.Infrastructure.Utils.Utils.Fail<PeerManager>($"Failed to get peer manager for cryptocode: {cryptoCode}");

        public PeerManager GetPeerManager(NRustLightningNetwork n) => GetPeerManager(n.CryptoCode);

        public IEnumerable<PeerManager> GetAll() => _peerManagers.Values;
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
                    var repo = _repositoryProvider.GetRepository(n);

                    /// TODO
                    uint currentBlockHeight = 0;

                    ChannelManager? maybeChanMan = null;
                    int tried = 0;
                    retry:
                    try
                    {
                        maybeChanMan = await repo.GetChannelManager(new ChannelManagerReadArgs(_keysRepository, b, feeEst, logger), cancellationToken);
                    }
                    catch when (tried < 4)
                    {
                        tried++;
                        await Task.Delay(800, cancellationToken);
                        goto retry;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug($"failed to resume ChannelManager. ({ex})");
                        // we tried enough. creating new one.
                    }

                    PeerManager peerMan;
                    if (maybeChanMan is null)
                    {
                        _logger.LogInformation($"Creating fresh PeerManager... for {n.CryptoCode}");
                        peerMan = PeerManager.Create(peerManSeed.AsSpan(), n.NBitcoinNetwork, conf, chainWatchInterface,
                            _keysRepository, b,
                            logger, feeEst, currentBlockHeight);
                    }
                    else
                    {
                        _logger.LogInformation($"resuming PeerManager for {n.CryptoCode}");
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