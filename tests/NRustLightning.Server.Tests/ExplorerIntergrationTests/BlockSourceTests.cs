using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerComposeFixture;
using DotNetLightning.Utils;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;
using NRustLightning.Infrastructure;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models;
using NRustLightning.Interfaces;
using NRustLightning.Server.Tests.Support;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.Tests.ExplorerIntergrationTests
{
    public class BlockSourceTests : IClassFixture<DockerFixture>
    {
        private class DummyChainListener : IChainListener
        {
            private readonly ITestOutputHelper? _helper;

            public DummyChainListener(Block rootBlock, uint rootHeight, ITestOutputHelper? helper = null)
            {
                _helper = helper;
                Blocks = new Stack<Block>();
                for (int i = 0; i < rootHeight; i++)
                {
                    Blocks.Push(Network.RegTest.Consensus.ConsensusFactory.CreateBlock());
                }

                Blocks.Push(rootBlock);
            }

            public Stack<Block> Blocks;
            public void BlockConnected(Block block, uint height, Primitives.LNOutPoint key = null)
            {
                _helper?.WriteLine($"Connecting block {block.Header.GetHash()} in height {height}");
                Assert.Equal(Blocks.Count, (int)height);
                Assert.Equal(block.Header.HashPrevBlock, Blocks.Peek().GetHash());
                Blocks.Push(block);
            }

            public void BlockDisconnected(BlockHeader header, uint height, Primitives.LNOutPoint key = null)
            {
                _helper?.WriteLine($"Disconnecting block {header.GetHash()} from {header.HashPrevBlock} in height {height}");
                Assert.Equal(Blocks.Count - 1, (int)height);
                Assert.True(Blocks.Peek().Header.ToBytes().SequenceEqual(header.ToBytes()));
                Blocks.Pop();
            }
        }

        private readonly DockerFixture _dockerFixture;
        private readonly ITestOutputHelper _output;

        private readonly ILoggerFactory _loggerFactory;
        private readonly ExplorerClient _cli; 
        public BlockSourceTests(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            _dockerFixture = dockerFixture;
            _output = output;
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _cli = _dockerFixture.StartExplorerFixtureAsync(_output, nameof(BlockSourceTests)).GetAwaiter().GetResult();
        }
        

        [Fact]
        [Trait("IntegrationTest", "ExplorerFixture")]
        public async Task ResumeChainListenerFromLowerFork()
        {
            // root -> A(invalidated)
            //      \
            //       B -> C
            // resume from A to C
            var c = _cli.RPCClient;
            var blockSource = new BitcoinRPCBlockSource(c);
            var rootBlock = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            var listener = new DummyChainListener(rootBlock.Block, (uint)rootBlock.Height, _output);
            
            var addr = await c.GetNewAddressAsync();
            await c.GenerateToAddressAsync(1, addr);
            var oldTipHash = await c.GetBestBlockHashAsync();
            var oldTip = await c.GetBlockAsync(oldTipHash, GetBlockVerbosity.WithFullTx);
            listener.BlockConnected(oldTip.Block, (uint)oldTip.Height);
            await c.InvalidateBlockAsync(oldTipHash);
            addr = await c.GetNewAddressAsync();
            await c.GenerateToAddressAsync(2, addr);
            var newTipHash = await c.GetBestBlockHashAsync();
            var newTip = await c.GetBlockAsync(newTipHash, GetBlockVerbosity.WithFullTx);
            Assert.True(oldTip.Height < newTip.Height);
            await listener.SyncChainListener(oldTipHash, newTip.Header, (uint)newTip.Height, new List<BlockHeaderData>(), blockSource, Network.RegTest, _loggerFactory.CreateLogger(nameof(ResumeChainListenerFromLowerFork)));
            Assert.True(listener.Blocks.Peek().ToBytes().SequenceEqual(newTip.Block.ToBytes()));
        }

        [Fact]
        [Trait("IntegrationTest", "ExplorerFixture")]
        public async Task ResumeChainListenerFromHigherFork()
        {
            // root -> A(invalidated) -> B -> C
            //      \
            //       D -> E
            // resume from C to E

            var c = _cli.RPCClient;
            var blockSource = new BitcoinRPCBlockSource(c);
            var rootBlock = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            var listener = new DummyChainListener(rootBlock.Block, (uint)rootBlock.Height, _output);

            GetBlockRPCResponse? oldTip = null;
            for (int i = 0; i < 3; i++)
            {
                await c.GenerateToOwnAddressAsync(1);
                oldTip = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
                listener.BlockConnected(oldTip.Block, (uint) oldTip.Height);
            }

            var nextBlockOfRoot = await c.GetBlockHeaderAsync(rootBlock.Height + 1);
            await c.InvalidateBlockAsync(nextBlockOfRoot.GetHash());
            await c.GenerateToOwnAddressAsync(2);
            var newTip = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            await listener.SyncChainListener(oldTip?.Header.GetHash(), newTip.Header, (uint)newTip.Height, new List<BlockHeaderData>(), blockSource, Network.RegTest, _loggerFactory.CreateLogger(nameof(ResumeChainListenerFromHigherFork)));
            Assert.True(listener.Blocks.Peek().ToBytes().SequenceEqual(newTip.Block.ToBytes()));
        }

        [Fact]
        [Trait("IntegrationTest", "ExplorerFixture")]
        public async Task ResumeChainListenerFromLower()
        {
            // root -> A -> B -> C
            // resume from A to C

            var c = _cli.RPCClient;
            var blockSource = new BitcoinRPCBlockSource(c);

            var rootBlock = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            var listener = new DummyChainListener(rootBlock.Block, (uint)rootBlock.Height, _output);
            GetBlockRPCResponse? oldTip = null;

            await c.GenerateToOwnAddressAsync(1);
            oldTip = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            listener.BlockConnected(oldTip.Block, (uint)oldTip.Height);

            await c.GenerateToOwnAddressAsync(2);

            var newTip = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            await listener.SyncChainListener(oldTip.Header.GetHash(), newTip.Header, (uint)newTip.Height, new List<BlockHeaderData>(), blockSource, Network.RegTest, _loggerFactory.CreateLogger(nameof(ResumeChainListenerFromHigherFork)));
            Assert.True(listener.Blocks.Peek().ToBytes().SequenceEqual(newTip.Block.ToBytes()));
        }

        [Fact]
        [Trait("IntegrationTest", "ExplorerFixture")]
        public async Task ResumeChainListenerFromHigher()
        {
            // root -> A -> B(invalidated) -> C
            // resume from C to A
            var c = _cli.RPCClient;
            var blockSource = new BitcoinRPCBlockSource(c);

            var rootBlock = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            
            var listener = new DummyChainListener(rootBlock.Block, (uint)rootBlock.Height, _output);
            GetBlockRPCResponse? oldTip = null;

            for (int i = 0; i < 3; i++)
            {
                await c.GenerateToOwnAddressAsync(1);
                oldTip = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
                listener.BlockConnected(oldTip.Block, (uint) oldTip.Height);
            }
            var blockB = await c.GetBlockHeaderAsync(rootBlock.Height + 2);
            await c.InvalidateBlockAsync(blockB.GetHash());
            
            var newTip = await c.GetBestBlockAsync(GetBlockVerbosity.WithFullTx);
            await listener.SyncChainListener(oldTip?.Header.GetHash(), newTip.Header, (uint)newTip.Height, new List<BlockHeaderData>(), blockSource, Network.RegTest, _loggerFactory.CreateLogger(nameof(ResumeChainListenerFromHigherFork)));
            Assert.True(listener.Blocks.Peek().ToBytes().SequenceEqual(newTip.Block.ToBytes()));
        }
    }
}