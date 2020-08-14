using System;
using NRustLightning.Interfaces;

namespace NRustLightning.Adaptors
{
    public sealed class ChannelManagerReadArgs
    {
        public ManyChannelMonitor ManyChannelMonitor { get; }
        private readonly IKeysInterface _keysInterface;
        private readonly IBroadcaster _broadcaster;
        private readonly IFeeEstimator _feeEstimator;
        private readonly ILogger _logger;
        private readonly IChainWatchInterface _chainWatchInterface;
        private readonly NBitcoin.Network _n;


        internal KeysInterfaceDelegatesHolder KeysInterface => new KeysInterfaceDelegatesHolder(_keysInterface);
        internal BroadcasterDelegatesHolder BroadCaster => new BroadcasterDelegatesHolder(_broadcaster, _n);
        internal FeeEstimatorDelegatesHolder FeeEstimator => new FeeEstimatorDelegatesHolder(_feeEstimator);
        internal LoggerDelegatesHolder LoggerDelegatesHolder => new LoggerDelegatesHolder(_logger);
        internal ChainWatchInterfaceConverter ChainWatchInterface => new ChainWatchInterfaceConverter(_chainWatchInterface);


        public ChannelManagerReadArgs(IKeysInterface keysInterface, IBroadcaster broadcaster, IFeeEstimator feeEstimator, ILogger logger, IChainWatchInterface chainWatchInterface, NBitcoin.Network n, ManyChannelMonitor manyChannelMonitor)
        {
            _keysInterface = keysInterface ?? throw new ArgumentNullException(nameof(keysInterface));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _feeEstimator = feeEstimator ?? throw new ArgumentNullException(nameof(feeEstimator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _chainWatchInterface = chainWatchInterface ?? throw new ArgumentNullException(nameof(chainWatchInterface));
            _n = n;
            ManyChannelMonitor = manyChannelMonitor ?? throw new ArgumentNullException(nameof(manyChannelMonitor));
        }
    }
}