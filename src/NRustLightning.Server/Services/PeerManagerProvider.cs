using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using static NRustLightning.RustLightningTypes.PrimitiveExtensions;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure;
using NRustLightning.Infrastructure.Configuration;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.P2P;

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
        private readonly EventAggregator _eventAggregator;
        private Dictionary<string, PeerManager> _peerManagers = new Dictionary<string, PeerManager>();
        private Dictionary<string, ManyChannelMonitor> _manyChannelMonitors = new Dictionary<string, ManyChannelMonitor>();
        private readonly MemoryPool<byte> _pool = MemoryPool<byte>.Shared;

        public PeerManagerProvider(
            INBXplorerClientProvider nbXplorerClientProvider,
            NRustLightningNetworkProvider networkProvider,
            IKeysRepository keysRepository,
            ILoggerFactory loggerFactory,
            ChannelProvider channelProvider,
            IOptions<Config> config,
            RepositoryProvider repositoryProvider,
            EventAggregator eventAggregator
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
            _eventAggregator = eventAggregator;
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
        
        public Dictionary<NRustLightningNetwork, uint> CurrentHeightsInStartup { get; } = new Dictionary<NRustLightningNetwork, uint>();

        public IEnumerable<PeerManager> GetAll() => _peerManagers.Values;

        public IEnumerable<(PeerManager, ManyChannelMonitor)> GetAllWithManyChannelMonitors()
        {
            foreach (var kv in _peerManagers)
            {
                yield return (kv.Value, _manyChannelMonitors.GetValueOrDefault(kv.Key));
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var n in _networkProvider.GetAll())
            {
                var nbx = _nbXplorerClientProvider.TryGetClient(n);
                if (nbx != null)
                {
                    var b = new NbXplorerBroadcaster(nbx, _loggerFactory.CreateLogger<NbXplorerBroadcaster>(), _eventAggregator);
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
                        _logger.LogCritical("Failed to get current height through nbx. Check you have set `exposerpc` option for nbxplorer");
                        throw;
                    }
                    
                    CurrentHeightsInStartup.TryAdd(n, currentBlockHeight);

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
                        _logger.LogDebug($"failed to resume ManyChannelMonitor. ({ex.Message})");
                        // we tried enough. creating new one.
                    }

                    using var blockSource = new BitcoinRPCBlockSource(nbx.RPCClient);
                    var currentBlockHeader = await nbx.RPCClient.GetBlockHeaderAsync((await nbx.RPCClient.GetBestBlockHashAsync()));
                    if (manyChannelMonitor is null)
                    {
                        _logger.LogInformation("Creating new ManyChannelMonitor...");
                        manyChannelMonitor = ManyChannelMonitor.Create(n.NBitcoinNetwork, chainWatchInterface, b, logger, feeEst);
                    }
                    // sync channel monitor to the current state.
                    else
                    {
                        _logger.LogInformation($"Syncing ChannelMonitor for {n.CryptoCode} ...");
                        await manyChannelMonitor.SyncChannelMonitor(
                            latestBlockHashes,
                            currentBlockHeader,
                            currentBlockHeight,
                            blockSource,
                            n.NBitcoinNetwork,
                            _loggerFactory.CreateLogger($"{nameof(PeerManagerProvider)}:{nameof(ManyChannelMonitor)}"),
                            cancellationToken
                        );
                    }
                    
                    var chanManItems = await repo.GetChannelManager(new ChannelManagerReadArgs(_keysRepository, b, feeEst, logger, chainWatchInterface, n.NBitcoinNetwork, manyChannelMonitor), cancellationToken);
                    ChannelManager chanMan;
                    var blockNotifier = BlockNotifier.Create(chainWatchInterface);
                    if (chanManItems is null)
                    {
                    
                        _logger.LogInformation($"Creating fresh ChannelManager for {n.CryptoCode} ...");
                        chanMan = ChannelManager.Create(n.NBitcoinNetwork, conf, chainWatchInterface, _keysRepository, logger, b, feeEst, currentBlockHeight, manyChannelMonitor);
                        blockNotifier.RegisterChannelManager(chanMan);
                    }
                    else
                    {
                        uint256 latestBlockHash;
                        (latestBlockHash, chanMan) = chanManItems.Value;
                        blockNotifier.RegisterChannelManager(chanMan);
                        // sync channel manager to the current state
                        _logger.LogInformation($"Syncing BlockNotifier... for {n.CryptoCode}");
                        await blockNotifier.SyncChainListener(latestBlockHash, currentBlockHeader, currentBlockHeight,
                            blockSource, n.NBitcoinNetwork,
                            _loggerFactory.CreateLogger($"{nameof(PeerManagerProvider)}:{nameof(BlockNotifier)}"), cancellationToken);
                    }
                    blockNotifier.RegisterManyChannelMonitor(manyChannelMonitor);

                    var peerManSeed = new byte[32];
                    RandomUtils.GetBytes(peerManSeed);
                    var graph = await repo.GetNetworkGraph(cancellationToken);
                    PeerManager peerMan;
                    if (graph is null)
                    {
                        peerMan =
                            PeerManager.Create(peerManSeed, conf, chainWatchInterface, logger,
                                _keysRepository.GetNodeSecret().ToBytes(), chanMan, blockNotifier);
                    }
                    else
                    {
                        peerMan =
                            PeerManager.Create(peerManSeed, conf, chainWatchInterface, logger,
                                _keysRepository.GetNodeSecret().ToBytes(), chanMan, blockNotifier, 30000, graph);
                        // Resume all previous connection. but only for public channels.
                        var knownChannels = chanMan.ListChannels(_pool);
                        var nodesToConnect =
                            graph.Nodes
                                .Where(node => node.Value.AnnouncementInfo != null)
                                .Where(node => knownChannels.Any(c => c.RemoteNetworkId.Equals(node.Key.Value)))
                                .Select(node => (node.Key.Value, node.Value.AnnouncementInfo.Value.Addresses));

                        foreach (var (pk, addresses) in nodesToConnect)
                        {
                            // We don't want to refer to P2PConnectionHandler here. It will cause circular deps.
                            // Thus instead of creating new outbound connection. Put params into work queue and delegate
                            // The actual processing to another class.
                            var chan = _channelProvider.GetOutboundConnectionRequestQueue(n);
                            await chan.Writer.WriteAsync(new PeerConnectionString(pk, ToSystemAddress(addresses[0])), cancellationToken);
                        }
                    }

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
                if (n.CryptoCode.ToLowerInvariant() == "btc")
                    Debug.Assert(repo != null);
                if (repo != null)
                {
                    await repo.SetManyChannelMonitor(_manyChannelMonitors[n.CryptoCode], cancellationToken);
                    await repo.SetChannelManager(_peerManagers[n.CryptoCode].ChannelManager, cancellationToken);
                    await repo.SetNetworkGraph(_peerManagers[n.CryptoCode].GetNetworkGraph(_pool), cancellationToken);
                }
            }
        }
    }
}