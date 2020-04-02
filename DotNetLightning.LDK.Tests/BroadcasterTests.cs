using System;
using DotNetLightning.LDK.Handles;
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
            Assert.Throws<ObjectDisposedException>(() => broadcaster.Broadcast());
        }

        [Fact]
        public void TestBroadcasterWrapper()
        {
            var wrapper = BroadcasterWrapper.Create();
            wrapper.Broadcast();
            wrapper.Dispose();
            Assert.Throws<ObjectDisposedException>(() => wrapper.Broadcast());
        }
    }
}