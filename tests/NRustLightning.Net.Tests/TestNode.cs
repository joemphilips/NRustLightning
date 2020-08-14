using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNetLightning.Peer;
using NBitcoin;
using NRustLightning.RustLightningTypes;
using NRustLightning.Tests.Common;
using NRustLightning.Tests.Common.Utils;
using NRustLightning.Utils;
using Xunit;

namespace NRustLightning.Net.Tests
{
    /// <summary>
    /// Thin wrapper for PeerManager to test its behavior with in-memory connection.
    /// </summary>
    public class TestNode : IDisposable
    {
        public readonly NodeConfig NodeConfig;
        private readonly TestUserConfigProvider _testUserConfigProvider;
        public PeerManager PeerManager { get; }
        public MemoryPool<byte> _pool = MemoryPool<byte>.Shared;
        public ChannelManager ChannelManager => PeerManager.ChannelManager;
        public BlockNotifier BlockNotifier => PeerManager.BlockNotifier;
        

        public async Task<Event[]> AwaitEvents()
        {
            Event[] events = {};
            var timeout = new CancellationTokenSource();
            timeout.CancelAfter(TimeSpan.FromSeconds(1));
            while (events.Length == 0)
            {
                timeout.Token.ThrowIfCancellationRequested();
                Assert.True(await EventNotify.Reader.WaitToReadAsync(timeout.Token));
                await EventNotify.Reader.ReadAsync(timeout.Token);
                events = ChannelManager.GetAndClearPendingEvents(_pool);
            }

            return events;
        }

        private SocketDescriptorFactory _socketDescriptorFactory = new SocketDescriptorFactory();
        
        public Channel<byte> EventNotify { get; }
        public TestNode(
            PeerManager peerManager,
            NodeConfig nodeConfig,
            Channel<byte> eventNotify
        )
        {
            NodeConfig = nodeConfig ?? throw new ArgumentNullException(nameof(nodeConfig));
            PeerManager = peerManager ?? throw new ArgumentNullException(nameof(peerManager));
            EventNotify = eventNotify;
        }

        private (ConnectionLoop, SocketDescriptor) CreateConnection(IDuplexPipe pipe)
        {
            var (socketDescriptor, writeReceiver) = _socketDescriptorFactory.GetNewSocket(pipe.Output);
            var conn = new ConnectionLoop(pipe, socketDescriptor, PeerManager, writeReceiver, EventNotify.Writer);
            return (conn, socketDescriptor);
        }

        internal async Task<(ConnectionLoop, ConnectionLoop)> ConnectTo(TestNode other)
        {
            var pipe = DuplexPipe.CreateConnection(PipeOptions.Default, PipeOptions.Default);
            var (conn, descriptor) = this.CreateConnection(pipe.Application);
            var (conn2, descriptor2) = other.CreateConnection(pipe.Transport);
            // manually start outbound connection from node A.
            var initialSend = PeerManager.NewOutboundConnection(descriptor, other.NodeConfig.KeysManager.GetNodeSecret().PubKey.ToBytes());
            await pipe.Application.Output.WriteAsync(initialSend);
            var flushResult = pipe.Application.Output.FlushAsync();
            if (!flushResult.IsCompleted)
            {
                await flushResult.ConfigureAwait(false);
            }
            
            // manually start inbound connection for node B.
            other.PeerManager.NewInboundConnection(descriptor2);
            
            conn.Start();
            conn2.Start();
            return (conn, conn2);
        }

        public void Dispose()
        {
            Assert.Empty(this.PeerManager.ChannelManager.GetAndClearPendingEvents(_pool));
            var networkGraph = this.PeerManager.GetNetworkGraph(_pool);
            var peerMan = PeerManager.Create(NodeConfig.NodeSeed.ToBytes(), _testUserConfigProvider, NodeConfig.ChainMonitor, NodeConfig.TestLogger, NodeConfig.KeysManager.GetNodeSecret().ToBytes(), ChannelManager, BlockNotifier, 30000, networkGraph);
            Assert.Equal(networkGraph, peerMan.GetNetworkGraph(_pool));
            
            _pool.Dispose();
            PeerManager.Dispose();
        }
    }

    public class TestNodeNetwork : IAsyncDisposable
    {
        public List<TestNode> Nodes;
        private readonly List<ConnectionLoop> _conns;

        public TestNode this[int index] => Nodes[index];

        public TestNodeNetwork(List<TestNode> nodes, List<ConnectionLoop> conns)
        {
            Nodes = nodes;
            _conns = conns;
        }
        public static async Task<TestNodeNetwork> CreateNetwork(IEnumerable<NodeConfig> configs)
        {
            var nodes = new List<TestNode>();
            foreach (var c in configs)
            {
                var blockNotifier = BlockNotifier.Create(c.ChainMonitor);
                blockNotifier.RegisterManyChannelMonitor(c.ManyChannelMonitor);
                var channelManager = ChannelManager.Create(c.N, c.ConfigProvider, c.ChainMonitor, c.KeysManager, c.TestLogger, c.TestBroadcaster, c.TestFeeEstimator, 0, c.ManyChannelMonitor);
                blockNotifier.RegisterChannelManager(channelManager);
                var peerManager = PeerManager.Create(c.NodeSeed.ToBytes().AsSpan(), c.ConfigProvider, c.ChainMonitor, c.TestLogger, c.KeysManager.GetNodeSecret().ToBytes(), channelManager, blockNotifier);
                nodes.Add(new TestNode(peerManager, c, Channel.CreateBounded<byte>(5)));
            }

            var connections = new List<ConnectionLoop>();
            foreach (var (a, b) in nodes.Combinations2())
            {
                var (c1, c2) = await a.ConnectTo(b);
                connections.Add(c1);
                connections.Add(c2);
            }
            return new TestNodeNetwork(nodes, connections);
        }

        public async Task<Transaction> CreateChannel(int from, int to)
        {
            var userId = RandomUtils.GetUInt64();
            Nodes[from].ChannelManager.CreateChannel(Nodes[to].NodeConfig.KeysManager.GetNodeSecret().PubKey, 100001U, 50000000, userId);
            Nodes[from].PeerManager.ProcessEvents();
            var e0 = await Nodes[from].AwaitEvents();
            Assert.Single(e0);
            Assert.True(e0[0].IsFundingGenerationReady);
            var item= ((Event.FundingGenerationReady) e0[0]).Item;
            var n = this.Nodes[0].NodeConfig.N;
            var txb = n.CreateTransactionBuilder();
            var ourFundKey = new Key();
            var previousTx = n.CreateTransaction();
            previousTx.Outputs.Add(Money.Satoshis(item.ChannelValueSatoshis), ourFundKey.PubKey.WitHash);
            txb.AddCoins(previousTx.Outputs.AsCoins().First());
            txb.AddKeys(ourFundKey);
            txb.SendAll(item.OutputScript);
            var fundingTx = txb.BuildTransaction(true);
            Nodes[from].ChannelManager.FundingTransactionGenerated(item.TemporaryChannelId.Value, fundingTx.Outputs.AsCoins().First().Outpoint);
            Nodes[from].PeerManager.ProcessEvents();
            
            var e1 = await Nodes[from].AwaitEvents();
            Assert.Single(e1);
            Assert.True(e1[0].IsFundingBroadcastSafe);
            return fundingTx;

        }

        public async ValueTask DisposeAsync()
        {
            foreach (var conn in _conns)
            {
                await conn.DisposeAsync();
            }

            foreach (var n in Nodes)
            {
                n.Dispose();
            }
        }
    }
}