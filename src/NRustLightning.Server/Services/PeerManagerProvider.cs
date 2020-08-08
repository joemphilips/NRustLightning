using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models;
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
        private Dictionary<string, ManyChannelMonitor> _manyChannelMonitors = new Dictionary<string, ManyChannelMonitor>();

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
            TryGetPeerManager(cryptoCode) ?? Infrastructure.Utils.Utils.Fail<PeerManager>($"Failed to get peer manager for cryptocode: {cryptoCode}");

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
                    var logger = new NativeLogger(_loggerFactory.CreateLogger<NativeLogger>());
                    var repo = _repositoryProvider.GetRepository(n);

                    uint currentBlockHeight;
                    int tried0 = 0;
                    retry0:
                    try
                    {
                        currentBlockHeight = (uint) await nbx.RPCClient.GetBlockCountAsync();
                    }
                    catch when (tried0 < 4)
                    {
                        tried0++;
                        await Task.Delay(1000, cancellationToken);
                        goto retry0;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical("Failed to get current height through nbx.");
                        throw;
                    }

                    ManyChannelMonitor? manyChannelMonitor = null;
                    Dictionary<Primitives.LNOutPoint, uint256> latestBlockHashes = null;
                    
                    int tried = 0;
                    retry1:
                    try
                    {
                        var items =
                            await repo.GetManyChannelMonitor(new ManyChannelMonitorReadArgs(chainWatchInterface, b, logger, feeEst, n.NBitcoinNetwork), cancellationToken);
                        if (items is null) throw new Exception("ManyChannelMonitor not found");
                        (manyChannelMonitor, latestBlockHashes) = items.Value;
                    }
                    catch when (tried < 4)
                    {
                        tried++;
                        await Task.Delay(800, cancellationToken);
                        goto retry1;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug($"failed to resume ManyChannelMonitor. ({ex})");
                        // we tried enough. creating new one.
                    }

                    var blockSource = new BitcoinRPCBlockSource(nbx.RPCClient);
                    var currentBlockHeader = await nbx.RPCClient.GetBlockHeaderAsync((await nbx.RPCClient.GetBestBlockHashAsync()));
                    if (manyChannelMonitor is null)
                    {
                        manyChannelMonitor = ManyChannelMonitor.Create(n.NBitcoinNetwork, chainWatchInterface, b, logger, feeEst);
                    }
                    // sync channel monitor to the current state.
                    else
                    {
                        await manyChannelMonitor.SyncChannelMonitor(latestBlockHashes, currentBlockHeader, currentBlockHeight, new List<BlockHeaderData>(), blockSource, n.NBitcoinNetwork, _logger);
                    }
                    
                    var chanManItems = await repo.GetChannelManager(new ChannelManagerReadArgs(_keysRepository, b, feeEst, logger, chainWatchInterface, n.NBitcoinNetwork, manyChannelMonitor), cancellationToken);
                    ChannelManager chanMan;
                    var blockNotifier = BlockNotifier.Create(chainWatchInterface);
                    if (chanManItems is null)
                    {
                    
                        _logger.LogInformation($"Creating fresh ChannelManager... for {n.CryptoCode}");
                        chanMan = ChannelManager.Create(n.NBitcoinNetwork, conf, chainWatchInterface, _keysRepository, logger, b, feeEst, currentBlockHeight, manyChannelMonitor);
                        blockNotifier.RegisterChannelManager(chanMan);
                    }
                    else
                    {
                        uint256 latestBlockHash;
                        (latestBlockHash, chanMan) = chanManItems.Value;
                        blockNotifier.RegisterChannelManager(chanMan);
                        // sync channel manager to the current state
                        await blockNotifier.SyncChainListener(latestBlockHash, currentBlockHeader, currentBlockHeight,
                            new List<BlockHeaderData>(), blockSource, n.NBitcoinNetwork, _logger);
                    }
                    blockNotifier.RegisterManyChannelMonitor(manyChannelMonitor);

                    var peerManSeed = new byte[32];
                    RandomUtils.GetBytes(peerManSeed);
                    var peerMan =
                        PeerManager.Create(peerManSeed, conf, chainWatchInterface, logger, _keysRepository.GetNodeSecret().ToBytes(), chanMan, blockNotifier);
                    _manyChannelMonitors.Add(n.CryptoCode, manyChannelMonitor);
                    _peerManagers.Add(n.CryptoCode, peerMan);
                }
                _logger.LogInformation("PeerManagerProvider started");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

            foreach (var n in _networkProvider.GetAll())
            {
                var repo = _repositoryProvider.TryGetRepository(n);
                if (repo != null)
                {
                    await repo.SetManyChannelMonitor(_manyChannelMonitors[n.CryptoCode], cancellationToken);
                    await repo.SetChannelManager(_peerManagers[n.CryptoCode].ChannelManager, cancellationToken);
                }
            }
        }
    }
}