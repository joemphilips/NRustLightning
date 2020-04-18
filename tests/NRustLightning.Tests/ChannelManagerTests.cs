using System;
using System.Linq;
using System.Text;
using System.Transactions;
using DotNetLightning.Serialize;
using DotNetLightning.Utils;
using NBitcoin;
using NBitcoin.DataEncoders;
using Xunit;
using NRustLightning;
using NRustLightning.Adaptors;
using NRustLightning.Facades;
using NRustLightning.Tests.Utils;
using Network = NRustLightning.Adaptors.Network;

namespace NRustLightning.Tests
{
    public class ChannelManagerTests
    {
        private static HexEncoder Hex = new NBitcoin.DataEncoders.HexEncoder();
        private static Key[] _keys =
        {
            new Key(Hex.DecodeData("0101010101010101010101010101010101010101010101010101010101010101")),
            new Key(Hex.DecodeData("0202020202020202020202020202020202020202020202020202020202020202")),
            new Key(Hex.DecodeData("0303030303030303030303030303030303030303030303030303030303030303")),
            new Key(Hex.DecodeData("0404040404040404040404040404040404040404040404040404040404040404")),
            new Key(Hex.DecodeData("0505050505050505050505050505050505050505050505050505050505050505")),
            new Key(Hex.DecodeData("0606060606060606060606060606060606060606060606060606060606060606")),
        };

        private static PubKey[] _pubKeys = _keys.Select(k => k.PubKey).ToArray();
        private static Primitives.NodeId[] _nodeIds = _pubKeys.Select(x => Primitives.NodeId.NewNodeId(x)).ToArray();


        private ChannelManager GetTestChannelManager()
        {
            
            var logger = new TestLogger();
            var broadcaster = new TestBroadcaster();
            var feeEstiamtor = new TestFeeEstimator();
            var chainWatchInterface = new TestChainWatchInterface();
            var seed = new byte[]{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }.AsSpan();
            var n = Network.TestNet;
            var channelManager = ChannelManager.Create(seed, in n, in TestUserConfig.Default, chainWatchInterface, logger, broadcaster, feeEstiamtor, 400000);
            return channelManager;
        }
        
        [Fact(Timeout = 15000)]
        public void CanCreateChannelManager()
        {
            var channelManager = GetTestChannelManager();
            var nodeFeature = FeatureBit.CreateUnsafe(0b000000100100000100000000);
            var channelFeature = FeatureBit.CreateUnsafe(0b000000100100000100000000);
            var hop1 = new RouteHopWithFeature(_nodeIds[0], nodeFeature, 1, channelFeature, 1000, 72);
            var hop2 = new RouteHopWithFeature(_nodeIds[1], nodeFeature, 2, channelFeature, 1000, 72);
            var route = new RouteWithFeature(hop1, hop2);
            
            var paymentHash = new uint256();
            var e = Assert.Throws<Exception>(() => channelManager.SendPayment(route, paymentHash.ToBytes()));
            Assert.Equal("FFI against rust-lightning failed (InternalError), Error: No channel available with first hop!", e.Message);
            channelManager.Dispose();
        }

        [Fact]
        public void CanGetPendingEvent()
        {
            var channelManager = GetTestChannelManager();
        }
    }
}