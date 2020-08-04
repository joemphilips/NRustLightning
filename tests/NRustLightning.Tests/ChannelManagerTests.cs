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
using NRustLightning.RustLightningTypes;
using NRustLightning.Tests.Utils;
using NRustLightning.Utils;
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
            var n = NBitcoin.Network.TestNet;
            var chainWatchInterface = new ChainWatchInterfaceUtil(n);
            var keySeed = new byte[]{ 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2 };
            var keysInterface = new KeysManager(keySeed, DateTime.UnixEpoch);
            var manyChannelMonitor = ManyChannelMonitor.Create(n, chainWatchInterface, broadcaster, logger, feeEstiamtor);
            var channelManager = ChannelManager.Create(n, in TestUserConfig.Default, chainWatchInterface, keysInterface, logger, broadcaster, feeEstiamtor, 400000, manyChannelMonitor);
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
            var e = Assert.Throws<PaymentSendException>(() => channelManager.SendPayment(routes, paymentHash.ToBytes()));
            Assert.Equal(PaymentSendFailureType.AllFailedRetrySafe, e.Kind);
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
            channelManager.CloseChannel(c[0].ChannelId);
            var events = channelManager.GetAndClearPendingEvents(_pool);
            Assert.Empty(events);
            channelManager.Dispose();
        }

        [Fact]
        public void ChannelManagerSerializationTests()
        {
            using var channelManager = GetTestChannelManager();
            var b = channelManager.Serialize(_pool);
            
            var keySeed = new byte[]{ 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2 };
            var keysInterface = new KeysManager(keySeed, DateTime.UnixEpoch);
            var logger = new TestLogger();
            var broadcaster = new TestBroadcaster();
            var feeEstiamtor = new TestFeeEstimator();
            var n = NBitcoin.Network.TestNet;
            var chainWatchInterface = new ChainWatchInterfaceUtil(n);
            var manyChannelMonitor =
                ManyChannelMonitor.Create(n, chainWatchInterface, broadcaster, logger, feeEstiamtor);
            var args = new ChannelManagerReadArgs(keysInterface, broadcaster, feeEstiamtor, logger, chainWatchInterface, n, manyChannelMonitor);
            var items = ChannelManager.Deserialize(b, args, new TestUserConfigProvider(), _pool);
            var (latestBlockhash, channelManager2) = items;

            Assert.True(channelManager2.Serialize(_pool).SequenceEqual(b));
        }
    }
}