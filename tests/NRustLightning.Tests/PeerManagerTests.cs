using System;
using System.Buffers;
using System.IO;
using System.Linq;
using DotNetLightning.Serialize;
using DotNetLightning.Utils;
using NBitcoin;
using NBitcoin.DataEncoders;
using NRustLightning.Adaptors;
using NRustLightning.Facades;
using NRustLightning.Tests.Utils;
using NRustLightning.Utils;
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
            var peerManager =
                PeerManager.Create(
                    seed, n, in TestUserConfig.Default, chainWatchInterface, keysInterface, broadcaster, logger, feeEstiamtor, 400000
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
            var e = Assert.Throws<FFIException>(() => channelManager.SendPayment(routes, paymentHash.ToBytes()));
            Assert.Equal("FFI against rust-lightning failed (InternalError), Error: AllFailedRetrySafe([No channel available with first hop!])", e.Message);

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
    }
}