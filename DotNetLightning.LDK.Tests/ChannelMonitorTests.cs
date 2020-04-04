using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class ChannelMonitorTests
    {
        
        [Fact]
        public void ChannelMonitorTest()
        {
            var chanmon = ChannelMonitor.Create(new TestChainWatchInterface(), new TestBroadcaster(), new TestLogger(), new TestFeeEstimator());
            chanmon.Dispose();
        }
    }
}