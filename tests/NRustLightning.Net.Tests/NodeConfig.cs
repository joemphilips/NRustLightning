using NBitcoin;
using NRustLightning.Interfaces;
using NRustLightning.Tests.Common.Utils;
using NRustLightning.Utils;

namespace NRustLightning.Net.Tests
{
    public class NodeConfig
    {
        public ChainWatchInterfaceUtil ChainMonitor;
        public TestBroadcaster TestBroadcaster;
        public TestFeeEstimator TestFeeEstimator;
        public ManyChannelMonitor ManyChannelMonitor;
        public TestLogger TestLogger;
        public uint256 NodeSeed;
    }
}