using DotNetLightning.LDK.Tests.Utils;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class BroadcasterTests
    {
        [Fact]
        public void TestBroadcaster()
        {
            var broadcaster = BroadcasterWrapper.Create();
            broadcaster.Dispose();
        }
    }
}