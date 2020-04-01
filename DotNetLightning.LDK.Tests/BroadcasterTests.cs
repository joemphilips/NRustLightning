using DotNetLightning.LDK.Tests.Utils;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class BroadcasterTests
    {
        [Fact]
        public void TestBroadcaster()
        {
            var broadcaster = Broadcaster.Create();
            broadcaster.Broadcast();
            broadcaster.Dispose();
        }
    }
}