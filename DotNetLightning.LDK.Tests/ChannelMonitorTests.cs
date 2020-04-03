using System;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class ChannelMonitorTests
    {
        [Fact]
        public void ChannelMonitorTest()
        {
            var chanmon = ChannelMonitor.Create();
            chanmon.Dispose();
        }
    }
}