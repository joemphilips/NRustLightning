using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetLightning.Utils;
using Microsoft.FSharp.Core;
using NBitcoin;
using NRustLightning.RustLightningTypes;
using Xunit;

namespace NRustLightning.Net.Tests
{
    public class UnitTest1
    {
        private MemoryPool<byte> _pool = MemoryPool<byte>.Shared;
        [Fact]
        public async Task CanConnectNodesAndGetChannelDetails()
        {
            var configs = Enumerable.Range(0, 2).Select(_ => NodeConfig.CreateDefault()).ToList();
            var nodes = await TestNodeNetwork.CreateNetwork(configs);
            var fundingTx = await nodes.CreateChannel(0, 1);
            
            var channels0 = nodes[0].ChannelManager.ListChannels(_pool);
            Assert.Single(channels0);
            var channels1 = nodes[1].ChannelManager.ListChannels(_pool);
            Assert.Single(channels1);
            Assert.False(channels0[0].IsLive);
            Assert.False(channels1[0].IsLive);
            Assert.Equal(channels0[0].RemoteNetworkId, nodes[1].NodeConfig.KeysManager.GetNodeSecret().PubKey);
            Assert.Equal(channels1[0].RemoteNetworkId, nodes[0].NodeConfig.KeysManager.GetNodeSecret().PubKey);
            // short channel id is not defined until funding tx gets confirmed.
            Assert.Equal(channels0[0].ShortChannelId, null);
            Assert.Equal(channels1[0].ShortChannelId, null);
            
            var block = configs[0].N.Consensus.ConsensusFactory.CreateBlock();
            block.Transactions = new List<Transaction>();
            block.Transactions.Add(fundingTx);
            nodes[0].BlockNotifier.BlockConnected(block, 1);
            nodes[1].BlockNotifier.BlockConnected(block, 1);

            channels0 = nodes[0].ChannelManager.ListChannels(_pool);
            channels1 = nodes[1].ChannelManager.ListChannels(_pool);
            var expectedChannelId = Primitives.ShortChannelId.TryParse("1x0x0").ResultValue;
            Assert.Equal(expectedChannelId, channels0[0].ShortChannelId);
            Assert.Equal(expectedChannelId, channels1[0].ShortChannelId);
            
            Assert.True(channels0[0].InboundCapacityMSat > 0);
            Assert.True(channels0[0].OutboundCapacityMSat > 0);
            Assert.True(channels1[0].InboundCapacityMSat > 0);
            Assert.True(channels1[0].OutboundCapacityMSat > 0);
            
            // Channel monitor deserialization tests.
            var monitor = nodes[0].NodeConfig.ManyChannelMonitor;
            var monitorB = monitor.Serialize(_pool);
            var (monitor2, latestBlockHashes) = ManyChannelMonitor.Deserialize(nodes[0].NodeConfig.ManyChannelMonitorReadArgs, monitorB, _pool);
            Assert.Single(latestBlockHashes);
            Assert.Equal(latestBlockHashes.Keys.First().Item, fundingTx.Outputs.AsCoins().First().Outpoint);
            Assert.Equal(latestBlockHashes.Values.First(), block.GetHash());
            
            // Channel manager deserialization tests.
            var managerB = nodes[0].ChannelManager.Serialize(_pool);
            var (latestBlockHash, manager2) = ChannelManager.Deserialize(managerB, nodes[0].NodeConfig.GetChannelManagerReadArgs(monitor), nodes[0].NodeConfig.ConfigProvider, _pool);
            Assert.Equal(block.GetHash(), latestBlockHash);
            Assert.Single(manager2.ListChannels(_pool));
            Assert.Equal(manager2.ListChannels(_pool)[0], channels0[0]);
        }
    }
}
