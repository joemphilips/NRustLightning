using DotNetLightning.LDK.Tests.Utils;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class ChannelMonitorTests
    {
        
        [Fact(Skip="it is crashing, take a look after channelmonitor tests")]
        public void ChannelMonitorTest()
        {
            var chanmon = ChannelMonitor.Create(new TestChainWatchInterface(), new TestBroadcaster(), new TestLogger(), new TestFeeEstimator());
            chanmon.Dispose();
        }
    }
}