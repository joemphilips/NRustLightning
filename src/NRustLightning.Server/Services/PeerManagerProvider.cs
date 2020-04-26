using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NRustLightning.Handles;
using NRustLightning.Interfaces;
using NRustLightning.Server.Configuration;
using NRustLightning.Server.FFIProxies;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Repository;

namespace NRustLightning.Server.Services
{
    public class PeerManagerProvider
    {
        private readonly IRPCClientProvider rpcClientProvider;
        private readonly NBXplorerClientProvider nbXplorerClientProvider;
        private Dictionary<string, PeerManager> _peerManagers = new Dictionary<string, PeerManager>();

        public PeerManagerProvider(
            IRPCClientProvider rpcClientProvider,
            NBXplorerClientProvider nbXplorerClientProvider,
            NRustLightningNetworkProvider networkProvider,
            IKeysRepository keysRepository,
            ILoggerFactory loggerFactory,
            IOptions<Config> config
            )
        {
            this.rpcClientProvider = rpcClientProvider;
            this.nbXplorerClientProvider = nbXplorerClientProvider;
            foreach (var n in networkProvider.GetAll())
            {
                var rpc = rpcClientProvider.GetRpcClient(n);
                var nbx = nbXplorerClientProvider.GetClient(n);
                if (!(rpc is null) && !(nbx is null))
                {
                    var b = new BitcoinCoreBroadcaster(rpc);
                    var feeEst = new BitcoinCoreFeeEstimator(rpc);
                    var chainWatchInterface = new NBXChainWatchInterface(nbx);
                    var logger = new NativeLogger(loggerFactory.CreateLogger<NativeLogger>());
                    var routingMsgHandler = new RoutingMsgHandler();
                    var seed = new byte[32];
                    RandomUtils.GetBytes(seed);
                    var ffiN = n.FFINetwork;
                    var conf = config.Value.RustLightningConfig;
                    var peerMan = PeerManager.Create(seed.AsSpan(), in ffiN, in conf, chainWatchInterface, b, logger, feeEst, routingMsgHandler, 400000, keysRepository.GetNodeSecret().ToBytes());
                    _peerManagers.Add(n.CryptoCode, peerMan);
                }
            }
        }

        public PeerManager? GetPeerManager(string cryptoCode)
        {
            _peerManagers.TryGetValue(cryptoCode, out var p);
            return p;
        }

        public PeerManager? GetPeerManager(NRustLightningNetwork n) => GetPeerManager(n.CryptoCode);
    }
}