using System;
using System.Buffers;
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

        private MemoryPool<byte> _pool;
        public ChannelManagerTests()
        {
            _pool = MemoryPool<byte>.Shared;
        }


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
        
        [Fact]
        public void CanCreateChannelManager()
        {
            using var channelManager = PeerManagerTests.getTestPeerManager().ChannelManager;
            var nodeFeature = FeatureBit.CreateUnsafe(0b000000100100000100000000);
            var channelFeature = FeatureBit.CreateUnsafe(0b000000100100000100000000);
            var hop1 = new RouteHopWithFeature(_nodeIds[0], nodeFeature, 1, channelFeature, 1000, 72);
            var hop2 = new RouteHopWithFeature(_nodeIds[1], nodeFeature, 2, channelFeature, 1000, 72);
            var route1 = new[] {hop1, hop2};
            var routes = new RoutesWithFeature(route1);
            
            var paymentHash = new uint256();
            var e = Assert.Throws<FFIException>(() => channelManager.SendPayment(routes, paymentHash.ToBytes()));
            Assert.Equal("FFI against rust-lightning failed (InternalError), Error: AllFailedRetrySafe([No channel available with first hop!])", e.Message);
            channelManager.Dispose();
        }

        [Fact]
        public void CanCreateAndCloseChannel()
        {
            using var channelManager = PeerManagerTests.getTestPeerManager().ChannelManager;
            var pk = _pubKeys[0];
            channelManager.CreateChannel(pk, 100000, 1000, UInt64.MaxValue);
            var c = channelManager.ListChannels(_pool);
            Assert.Single(c);
            Assert.Equal(c[0].RemoteNetworkId, pk);
            Assert.Equal(100000U, c[0].ChannelValueSatoshis);
            // Before fully open, It must be 0.
            Assert.Equal(0U, c[0].InboundCapacityMSat);
            Assert.Equal(0U, c[0].OutboundCapacityMSat);
            
            Assert.False(c[0].IsLive);
            var e = Assert.Throws<FFIException>(() => channelManager.CloseChannel(c[0].ChannelId));
            Assert.Contains("No such channel", e.ToString());
            var events = channelManager.GetAndClearPendingEvents(_pool);
            channelManager.Dispose();
        }
    }
}