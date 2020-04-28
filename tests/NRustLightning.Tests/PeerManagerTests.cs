using System;
using System.Linq;
using DotNetLightning.Utils;
using NBitcoin;
using NBitcoin.DataEncoders;
using NRustLightning.Tests.Utils;
using Xunit;
using Network = NRustLightning.Adaptors.Network;

namespace NRustLightning.Tests
{
    public class PeerManagerTests
    {
        private static HexEncoder Hex = new NBitcoin.DataEncoders.HexEncoder();
        private static Key[] _keys =
        {
            new Key(Hex.DecodeData("0101010101010101010101010101010101010101010101010101010101010101")),
            new Key(Hex.DecodeData("0202020202020202020202020202020202020202020202020202020202020202")),
        };

        private static PubKey[] _pubKeys = _keys.Select(k => k.PubKey).ToArray();
        private static Primitives.NodeId[] _nodeIds = _pubKeys.Select(x => Primitives.NodeId.NewNodeId(x)).ToArray();

        private PeerManager getTestPeerManager()
        {
            
            var logger = new TestLogger();
            var broadcaster = new TestBroadcaster();
            var feeEstiamtor = new TestFeeEstimator();
            var chainWatchInterface = new TestChainWatchInterface();
            var seed = new byte[]{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }.AsSpan();
            var n = Network.TestNet;
            var routingMsgHandler = new TestRoutingMsgHandler();
            var ourNodeSecret = _keys[0].ToBytes();
            var peerManager =
                PeerManager.Create(
                    seed, in n, in TestUserConfig.Default, chainWatchInterface, broadcaster, logger, feeEstiamtor,  routingMsgHandler,400000, ourNodeSecret
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
            peerMan.Dispose();
        }
    }
}