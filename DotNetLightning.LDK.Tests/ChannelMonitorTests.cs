using DotNetLightning.LDK.Tests.Utils;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class ChannelMonitorTests
    {
        
        /// <summary>
        ///  TODO: Seems this crashes sometimes, must check the cause.
        /// </summary>
        [Fact(Skip = "skip channel monitor for now")]
        public void ChannelMonitorTest()
        {
            var chanmon = ChannelMonitor.Create(new TestChainWatchInterface(), new TestBroadcaster(), new TestLogger(), new TestFeeEstimator());
            chanmon.Dispose();
        }
    }
}