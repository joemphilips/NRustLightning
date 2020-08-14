using System;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Tests.Common.Utils;
using NRustLightning.Utils;
using Network = NBitcoin.Network;

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
        public KeysManager KeysManager;
        
        public IUserConfigProvider ConfigProvider;
        public Network N;

        public static int index = 0;
        public static ConsoleColor[] Colors = {ConsoleColor.White, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.DarkCyan, ConsoleColor.Gray, ConsoleColor.DarkGreen, ConsoleColor.DarkYellow};
        
        public ManyChannelMonitorReadArgs ManyChannelMonitorReadArgs => new ManyChannelMonitorReadArgs(ChainMonitor, TestBroadcaster, TestLogger, TestFeeEstimator, N);
        public ChannelManagerReadArgs GetChannelManagerReadArgs(ManyChannelMonitor manyChannelMonitor) =>
            new ChannelManagerReadArgs(KeysManager, TestBroadcaster, TestFeeEstimator, TestLogger, ChainMonitor, N, manyChannelMonitor);

        public static NodeConfig CreateDefault()
        {
            var n = Network.RegTest;
            var chainMonitor = new ChainWatchInterfaceUtil(n);
            var b = new TestBroadcaster();
            var l = new TestLogger(Colors[index]);
            index++;
            if (index == 8)
                index = 0;
            var est = new TestFeeEstimator();
            return new NodeConfig()
            {
                ChainMonitor = chainMonitor,
                TestBroadcaster = b,
                TestFeeEstimator = est,
                ManyChannelMonitor = ManyChannelMonitor.Create(n, chainMonitor, b, l, est),
                TestLogger = l,
                NodeSeed = RandomUtils.GetUInt256(),
                KeysManager = new KeysManager(RandomUtils.GetBytes(32), DateTime.Now),
                ConfigProvider = new TestUserConfigProvider(),
                N = n,
            };
        }
    }
}