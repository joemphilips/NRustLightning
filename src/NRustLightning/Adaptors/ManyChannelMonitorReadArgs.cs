using System;
using NRustLightning.Interfaces;

namespace NRustLightning.Adaptors
{
    public sealed class ManyChannelMonitorReadArgs
    {
        private readonly IChainWatchInterface _chainWatchInterface;
        private readonly IBroadcaster _broadcaster;
        private readonly ILogger _logger;
        private readonly IFeeEstimator _feeEstimator;
        private readonly NBitcoin.Network _n;

        internal ChainWatchInterfaceDelegatesHolder ChainWatchInterface => new ChainWatchInterfaceDelegatesHolder(_chainWatchInterface);
        internal BroadcasterDelegatesHolder Broadcaster => new BroadcasterDelegatesHolder(_broadcaster, _n);
        internal LoggerDelegatesHolder Logger => new LoggerDelegatesHolder(_logger);
        internal FeeEstimatorDelegatesHolder FeeEstimator => new FeeEstimatorDelegatesHolder(_feeEstimator);

        public ManyChannelMonitorReadArgs(IChainWatchInterface chainWatchInterface, IBroadcaster broadcaster,
            ILogger logger, IFeeEstimator feeEstimator, NBitcoin.Network n)
        {
            _chainWatchInterface = chainWatchInterface ?? throw new ArgumentNullException(nameof(chainWatchInterface));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _feeEstimator = feeEstimator ?? throw new ArgumentNullException(nameof(feeEstimator));
            _n = n ?? throw new ArgumentNullException(nameof(n));
        }
    }
}