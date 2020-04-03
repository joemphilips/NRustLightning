using System;
using System.Linq;
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
            // should not throw error.
            broadcaster.Broadcast();
            
            // running GC should not change the behavior
            for (var i = 0; i <= 1000000; i++) {var _garbage = new object();} // lots of garbage
            GC.Collect();
            GC.WaitForPendingFinalizers();
            broadcaster.Broadcast();
            
            broadcaster.Dispose();
            Assert.Throws<ObjectDisposedException>(() => broadcaster.Broadcast());
        }

        [Fact]
        public void TestBroadcasterWrapper()
        {
            var wrapper = BroadcasterWrapper.Create();
            wrapper.Broadcast();
            
            // running GC should not change the behavior
            for (var i = 0; i <= 1000000; i++) {var _garbage = new object();} // lots of garbage
            GC.Collect();
            GC.WaitForPendingFinalizers();
            wrapper.Broadcast();
            
            wrapper.Dispose();
            Assert.Throws<ObjectDisposedException>(() => wrapper.Broadcast());
        }
    }
}