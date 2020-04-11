using System;
using System.Text;
using System.Transactions;
using Xunit;
using DotNetLightning.LDK;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Tests.Utils;

namespace DotNetLightning.LDK.Tests
{
    public class ChannelManagerTests
    {
        [Fact]
        public void CanCreateChannelManager()
        {
            var logger = new TestLogger();
            var broadcaster = new TestBroadcaster();
            var feeEstiamtor = new TestFeeEstimator();
            var chainWatchInterface = new TestChainWatchInterface();
            var seed = new byte[]{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }.AsSpan();
            var n = Network.TestNet;
            var channelManager = ChannelManager.Create(seed, in n, in TestUserConfig.Default, chainWatchInterface, logger, broadcaster, feeEstiamtor, 400000);

            var hop1 = new FFIRoute();
            var hop2 = new FFIRoute();
            
            var paymentHash = new FFISha256dHash();
            //channelManager.SendPayment(new [] {hop1, hop2}, ref paymentHash);
            
            channelManager.Dispose();
        }
    }
}