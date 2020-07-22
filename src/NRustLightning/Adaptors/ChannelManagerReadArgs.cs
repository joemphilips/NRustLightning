using System;
using NRustLightning.Interfaces;

namespace NRustLightning.Adaptors
{
    public class ChannelManagerReadArgs
    {
        public IKeysInterface KeysInterface { get; }
        public IBroadcaster Broadcaster { get; }
        public IFeeEstimator FeeEstimator { get; }
        public ILogger Logger { get; }

        public ChannelManagerReadArgs(IKeysInterface keysInterface, IBroadcaster broadcaster, IFeeEstimator feeEstimator, ILogger logger)
        {
            KeysInterface = keysInterface ?? throw new ArgumentNullException(nameof(keysInterface));
            Broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            FeeEstimator = feeEstimator ?? throw new ArgumentNullException(nameof(feeEstimator));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}