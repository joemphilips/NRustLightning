using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerComposeFixture;
using DotNetLightning.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NBitcoin;
using NBitcoin.RPC;
using NRustLightning.Infrastructure;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models;
using NRustLightning.Interfaces;
using Xunit;
using Xunit.Abstractions;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.Tests
{
    public class BlockSourceTests : IClassFixture<DockerFixture>
    {
        private readonly DockerFixture _dockerFixture;
        private readonly ITestOutputHelper _output;

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

        public BlockSourceTests(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            _dockerFixture = dockerFixture;
            _output = output;
        }

        [Fact]
        [Trait("UnitTest", "UnitTest")]
        public async Task ForkStepTests()
        {
            // root -> A(invalidated)
            //      \
            //       B -> C
            // resume from A to C
            var cli = await _dockerFixture.StartExplorerFixtureAsync(_output, nameof(ForkStepTests));
            var c = cli.RPCClient;
            var blockSource = new BitcoinRPCBlockSource(c);
            var rootBlock = await c.GetBlockAsync(await c.GetBestBlockHashAsync(), GetBlockVerbosity.WithFullTx);
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
            var lf = LoggerFactory.Create(builder => builder.AddConsole());
            await listener.SyncChainListener(oldTipHash, newTip.Header, (uint)newTip.Height, new List<BlockHeaderData>(), blockSource, Network.RegTest, lf.CreateLogger(nameof(ForkStepTests)));
            Assert.True(listener.Blocks.Peek().ToBytes().SequenceEqual(newTip.Block.ToBytes()));
        }
    }
}