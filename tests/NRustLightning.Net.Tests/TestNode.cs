using System;
using System.Diagnostics;
using NRustLightning.Tests.Common.Utils;
using NRustLightning.Utils;

namespace NRustLightning.Net.Tests
{
    public class TestNode : IDisposable
    {
        public BlockNotifier BlockNotifier { get; }
        public TestNode(
            BlockNotifier blockNotifier,
            ChainWatchInterfaceUtil ChainMonitor,
            TestBroadcaster testBroadcaster,
            ManyChannelMonitor manyChannelMonitor
        )
        {
            BlockNotifier = blockNotifier ?? throw new ArgumentNullException(nameof(blockNotifier));
        }
        public static void CreateNetwork() {}

        public void Dispose()
        {
        }
    }
}