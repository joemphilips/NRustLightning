using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetLightning.Serialize;
using DotNetLightning.Utils;
using NBitcoin;
using NBitcoin.DataEncoders;
using NRustLightning.Adaptors;
using NRustLightning.Facades;
using NRustLightning.Tests.Common.Utils;
using NRustLightning.Utils;
using RustLightningTypes;
using Xunit;
using Xunit.Abstractions;
using Network = NRustLightning.Adaptors.Network;

namespace NRustLightning.Tests
{
    public class PeerManagerTests
    {
        private readonly ITestOutputHelper _output;
        private static HexEncoder Hex = new NBitcoin.DataEncoders.HexEncoder();
        private static Key[] _keys =
        {
            new Key(Hex.DecodeData("0101010101010101010101010101010101010101010101010101010101010101")),
            new Key(Hex.DecodeData("0202020202020202020202020202020202020202020202020202020202020202")),
        };
        
        private const uint TEST_FINAL_CTLV = 32u;

        private static PubKey[] _pubKeys = _keys.Select(k => k.PubKey).ToArray();
        private static Primitives.NodeId[] _nodeIds = _pubKeys.Select(x => Primitives.NodeId.NewNodeId(x)).ToArray();

        private MemoryPool<byte> _pool;
        
        public PeerManagerTests(ITestOutputHelper output)
        {
            _output = output;
            _pool = MemoryPool<byte>.Shared;
        }

        public static PeerManager getTestPeerManager()
        {
            
            var logger = new TestLogger();
            var broadcaster = new TestBroadcaster();
            var feeEstiamtor = new TestFeeEstimator();
            var n = NBitcoin.Network.TestNet;
            
            var chainWatchInterface = new ChainWatchInterfaceUtil(n);
            var seed = new byte[]{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 };
            var keysInterface = new KeysManager(seed, DateTime.UnixEpoch);
            var blockNotifier = BlockNotifier.Create(chainWatchInterface);
            var manyChannelMonitor = ManyChannelMonitor.Create(n, chainWatchInterface, broadcaster, logger, feeEstiamtor);
            var channelManager = ChannelManager.Create(n, TestUserConfig.Default, chainWatchInterface, keysInterface, logger, broadcaster, feeEstiamtor, 400000, manyChannelMonitor);
            blockNotifier.RegisterChannelManager(channelManager);
            blockNotifier.RegisterManyChannelMonitor(manyChannelMonitor);
            var peerManager =
                PeerManager.Create(
                    seed, in TestUserConfig.Default, chainWatchInterface, logger, keysInterface.GetNodeSecret().ToBytes(), channelManager, blockNotifier, 10000
                    );
            return peerManager;
        }
        
        [Fact]
        public void PeerManagerTestsSimple()
        {
            var socketFactory = new SocketDescriptorFactory();
            using var peerMan = getTestPeerManager();
            var socket1 = socketFactory.GetNewSocket();
            peerMan.NewInboundConnection(socket1);
            var socket2 = socketFactory.GetNewSocket();
            var theirNodeId = _pubKeys[1];
            var theirNodeIds = peerMan.GetPeerNodeIds(_pool);
            Assert.Empty(theirNodeIds);
            var actOne = peerMan.NewOutboundConnection(socket2, theirNodeId.ToBytes());
            Assert.Equal(50, actOne.Length);
            // Console.WriteLine($"actOne in C# is {Hex.EncodeData(actOne)}");

            theirNodeIds = peerMan.GetPeerNodeIds(_pool);
            // It does not count when handshake is not complete
            Assert.Empty(theirNodeIds);
            peerMan.Dispose();
        }

        [Fact]
        public void CanCreateAndDisposePeerManagerSafely()
        {
            for (int i = 0; i < 100; i++)
            {
                using var peerMan = getTestPeerManager();
                peerMan.Dispose();
            }
        }
        
        [Fact]
        public void CanCallChannelManagerThroughPeerManager()
        {
            using var peerMan = getTestPeerManager();
            
            var channelManager = peerMan.ChannelManager;
            var nodeFeature = FeatureBit.CreateUnsafe(0b000000100100000100000000);
            var channelFeature = FeatureBit.CreateUnsafe(0b000000100100000100000000);
            var hop1 = new RouteHopWithFeature(_nodeIds[0], nodeFeature, 1, channelFeature, 1000, 72);
            var hop2 = new RouteHopWithFeature(_nodeIds[1], nodeFeature, 2, channelFeature, 1000, 72);
            var route1 = new[] {hop1, hop2};
            var routes = new RoutesWithFeature(route1);
            
            var paymentHash = new uint256();
            var e = Assert.Throws<PaymentSendException>(() => channelManager.SendPayment(routes, paymentHash.ToBytes()));
            Assert.Equal(PaymentSendFailureType.AllFailedRetrySafe, e.Kind);

            peerMan.Dispose();
        }

        [Fact]
        public void CanCallBLockNotifierThroughPeerManager()
        {
            var peerMan = getTestPeerManager();
            var block = NBitcoin.Block.Parse(File.ReadAllText(Path.Join("Data", "block-testnet-828575.txt")), NBitcoin.Network.TestNet);
            peerMan.BlockNotifier.BlockConnected(block, 400001);
            peerMan.BlockNotifier.BlockConnected(block, 400003);
            peerMan.BlockNotifier.BlockDisconnected(block.Header, 400001);
            peerMan.Dispose();
        }

        [Fact]
        public void CanCallSendPayment()
        {
            using var peerMan = getTestPeerManager();
            var paymentHash = Primitives.PaymentHash.NewPaymentHash(uint256.Parse("4141414141414141414141414141414141414141414141414141414141414142"));
            var lastHops = new List<RouteHint>();
            var e = Assert.Throws<FFIException>(()  => peerMan.SendPayment(_keys[0].PubKey, paymentHash, lastHops, LNMoney.MilliSatoshis(100L), TEST_FINAL_CTLV, _pool));
            Assert.Contains( "Cannot route when there are no outbound routes away from us",e.Message);
            
            var secret = uint256.Parse("4141414141414141414141414141414141414141414141414141414141414143");
            e = Assert.Throws<FFIException>(() => peerMan.SendPayment(_keys[0].PubKey, paymentHash, lastHops, LNMoney.MilliSatoshis(100L), TEST_FINAL_CTLV, _pool, secret));
            Assert.Contains( "Cannot route when there are no outbound routes away from us",e.Message);
        }

    }
}