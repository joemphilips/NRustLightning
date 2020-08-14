using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Utils;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.Infrastructure.Models;
using NRustLightning.Infrastructure.Utils;
using NRustLightning.Interfaces;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NRustLightning.Infrastructure.Interfaces
{
    public interface IBlockSource
    {
        Task<BlockHeaderData> GetHeader(uint256 headerHash, uint? heightHint = null, CancellationToken ct = default);
        Task<Block> GetBlock(uint256 headerHash, CancellationToken ct = default);
        Task<Block> GetBestBlock(CancellationToken ct = default);
    }


    internal enum ForkStepKind
    {
        ForkPoint,
        DisconnectBlock,
        ConnectBlock
    }

    internal class ForkStep
    {
        public ForkStepKind Kind { get; }
        
        public BlockHeaderData HeaderData { get; }

        internal ForkStep(ForkStepKind kind, BlockHeaderData header)
        {
            Kind = kind;
            HeaderData = header ?? throw new ArgumentNullException(nameof(header));
        }
    }

    public static class BlockSourceExtension
    {
        private static uint256 ToUInt256(this System.Numerics.BigInteger input)
        {
            if (input.Sign < 1)
                throw new ArgumentException(nameof(input), "input should not be negative");

            var arr = input.ToByteArray();
            int len = arr.Length;
            if (arr[^1] == 0)
                len--;
            
            if (len > 32)
                throw new ArgumentException(nameof(input), "input is too big");
            
            if (len == 32)
                return new uint256(arr, 0, len);
            
            Span<byte> tmp = stackalloc byte[32];
            tmp.Clear();
            arr.AsSpan().Slice(0, len).CopyTo(tmp);
            return new uint256(tmp);
        }
        
        private static BigInteger GetWork(this Target bits)
        {
            // 2 ** 256 / (target + 1) == ~target / (target + 1) + 1
            // The equation is defined in bitcoind
            var ret = ~bits.ToBigInteger();
            BigInteger ret1 = bits.ToBigInteger();
            ret1++;
            ret = BigInteger.Divide(ret, ret1);
            ret++;
            return ret;
        }
        /// <summary>
        ///  Check that child header builds on previousHeader -- the claimed work differential matches the actual PoW
        /// in childHeader and the difficulty transition is possible, ie within 4x. Includes stateless header checks on
        /// previous header.
        /// </summary>
        /// <param name="childHeader"></param>
        /// <param name="previousHeader"></param>
        /// <param name="checkPoW"></param>
        /// <returns></returns>
        private static void CheckBuildsOn(BlockHeaderData childHeader, BlockHeaderData previousHeader, Network n)
        {
            if (childHeader.Header.HashPrevBlock != previousHeader.Header.GetHash())
            {
                throw new NRustLightningException($"Bogus block header (childHeader's previous block is: {childHeader.Header.HashPrevBlock}. actual previous block is {previousHeader.Header.GetHash()}");
            }
            
            // stateless check
            if (!previousHeader.Header.CheckProofOfWork())
            {
                throw new NRustLightningException("Bogus block header. Failed to verify PoW");
            }

            if (previousHeader.Height != childHeader.Height - 1)
            {
                throw new NRustLightningException($"Bogus block header previous header's height is {previousHeader.Height}. childHeader's height is {childHeader.Height}");
            }
            var newWork = childHeader.Header.Bits.GetWork();
            var workAccm = previousHeader.ChainWork.ToBigInteger() + newWork;
            if (workAccm != childHeader.ChainWork.ToBigInteger())
            {
                throw new NRustLightningException("Bogus block header");
            }

            if (n == Network.Main)
            {
                if (childHeader.Height % n.Consensus.DifficultyAdjustmentInterval == 0)
                {
                    var prevWork = previousHeader.Header.Bits.GetWork();
                    // The difficulty shouldn't change too drastically, it implies a network division.
                    if (newWork > prevWork << 2 || newWork < prevWork >> 2)
                    {
                        throw new NRustLightningException("Bogus block header");
                    }
                }
                // difficulty should change only the block height is multiple of 2016
                else if (childHeader.Header.Bits != previousHeader.Header.Bits)
                {
                    throw new NRustLightningException("Bogus block header");
                }
            }
        }
        
        internal static async Task FindForkStep(this IBlockSource blockSource, IList<ForkStep> stepsTx,
            BlockHeaderData currentHeader, BlockHeaderData prevHeader, Network n,
            CancellationToken ct = default)
        {
            while (true)
            {
                if (prevHeader.Header.HashPrevBlock == currentHeader.Header.HashPrevBlock)
                {
                    // found the fork, get the fork point header and we're done.
                    stepsTx.Add(new ForkStep(ForkStepKind.DisconnectBlock, prevHeader));
                    stepsTx.Add(new ForkStep(ForkStepKind.ConnectBlock, currentHeader));
                    BlockHeaderData newPrevHeader;
                    newPrevHeader = await blockSource.GetHeader(prevHeader.Header.HashPrevBlock, prevHeader.Height - 1, ct);
                    CheckBuildsOn(prevHeader, newPrevHeader, n);

                    stepsTx.Add(new ForkStep(ForkStepKind.ForkPoint, newPrevHeader.Clone()));
                    break;
                }

                if (currentHeader.Height == 0)
                {
                    throw new NRustLightningException("Bogus block data");
                }

                // if the current is higher than prev, we must connect the block (possibly after disconnecting until to
                // the fork point) so that prev can catch up.
                if (prevHeader.Height < currentHeader.Height)
                {
                    if (prevHeader.Height + 1 == currentHeader.Height &&
                        prevHeader.Header.GetHash() == currentHeader.Header.HashPrevBlock)
                    {
                        // we have resumed from the previous tip to the current tip! we are done.
                        stepsTx.Add(new ForkStep(ForkStepKind.ConnectBlock, currentHeader));
                        break;
                    }
                    else
                    {
                        // current is higher than the prev, walk current down by listing blocks we need to connect.
                        var newCurrentHeader = await blockSource.GetHeader(currentHeader.Header.HashPrevBlock, currentHeader.Height - 1, ct);
                        CheckBuildsOn(currentHeader, newCurrentHeader, n);
                        stepsTx.Add(new ForkStep(ForkStepKind.ConnectBlock, currentHeader.Clone()));
                        currentHeader = newCurrentHeader;
                    }
                }
                // previous header is higher, walk it back and recurse.
                else if (prevHeader.Height > currentHeader.Height)
                {
                    stepsTx.Add(new ForkStep(ForkStepKind.DisconnectBlock, prevHeader.Clone()));
                    var newPrevHeader =
                        await blockSource.GetHeader(prevHeader.Header.HashPrevBlock, (prevHeader.Height - 1), ct);
                    CheckBuildsOn(prevHeader, newPrevHeader, n);
                    prevHeader = newPrevHeader;
                }
                else
                {
                    // Target and current at the same height, but we're not at fork yet, walk both back and recurse
                    var newCurrentHeader = await blockSource.GetHeader(currentHeader.Header.HashPrevBlock, currentHeader.Height - 1, ct);
                    CheckBuildsOn(currentHeader, newCurrentHeader, n);
                    stepsTx.Add(new ForkStep(ForkStepKind.ConnectBlock, currentHeader.Clone()));
                    stepsTx.Add(new ForkStep(ForkStepKind.DisconnectBlock, prevHeader.Clone()));
                    currentHeader = newCurrentHeader;

                    var newPrevHeader =
                        await blockSource.GetHeader(prevHeader.Header.HashPrevBlock, prevHeader.Height - 1, ct);
                    CheckBuildsOn(prevHeader, newPrevHeader, n);
                    prevHeader = newPrevHeader;
                }
            }
        }

        private static async Task<List<ForkStep>> FindFork(this IBlockSource blockSource, BlockHeaderData currentHeader, BlockHeaderData prevHeader, Network n)
        {
            var stepsTx = new List<ForkStep>();
            if (currentHeader.Equals(prevHeader))
            {
                return stepsTx;
            }

            await blockSource.FindForkStep(stepsTx, currentHeader, prevHeader, n);
            return stepsTx;
        }

        private static async Task SyncOneChannelListener(this IChainListener chainListener, Primitives.LNOutPoint? maybeKey,
            uint256 oldBlockHash, BlockHeader newBlockHeader, uint currentHeight,
            IBlockSource blockSource, Network n, ILogger? logger = null
        )
        {
            var oldHeaderData = await blockSource.GetHeader(oldBlockHash);
            var newHeaderData = new BlockHeaderData(currentHeight, newBlockHeader);
            var events = await blockSource.FindFork(newHeaderData, oldHeaderData, n);
                
            uint256? lastDisconnectTip = null;
            BlockHeaderData? newTip = null;
            foreach (var e in events)
            {
                switch (e.Kind)
                {
                    case ForkStepKind.DisconnectBlock:
                        logger?.LogDebug($"DisConnecting block_header {e.HeaderData.Header.GetHash()} in height: {e.HeaderData.Height}");
                        chainListener.BlockDisconnected(e.HeaderData.Header, e.HeaderData.Height, maybeKey);
                        lastDisconnectTip = e.HeaderData.Header.HashPrevBlock;
                        break;
                    case ForkStepKind.ForkPoint:
                        newTip = e.HeaderData.Clone();
                        break;
                    case ForkStepKind.ConnectBlock:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var isBothNull = lastDisconnectTip is null && newTip is null;
            var isBothHaveValue = lastDisconnectTip != null && newTip != null;
            // If we disconnected any blocks, we should have new tip data available, which should match our cached
            // header data if it is available. If we didn't disconnect any blocks we shouldn't have set a ForkPoint
            // as there is no fork.
            Debug.Assert(isBothNull || isBothHaveValue);

            events.Reverse();
            foreach (var headerData in from e in events where e.Kind == ForkStepKind.ConnectBlock select e.HeaderData)
            {
                var block = await blockSource.GetBlock(headerData.Header.GetHash());
                if (block.Header.GetHash() != headerData.Header.GetHash() || !block.CheckMerkleRoot())
                    throw new NRustLightningException("Bogus header data");
                
                logger?.LogDebug($"Connecting block {headerData.Header.GetHash()}");
                chainListener.BlockConnected(block, headerData.Height, maybeKey);
                newTip = headerData;
            }
        }

        public static Task SyncChainListener(this IChainListener chainListener, uint256 oldLatestBlockHash,
            BlockHeader newBlockHeader, uint currentHeight, IBlockSource blockSource,
            Network n, ILogger? logger = null)
        {
            return SyncOneChannelListener(chainListener, null, oldLatestBlockHash, newBlockHeader, currentHeight,
                blockSource, n, logger);
        }
        public static async Task SyncChannelMonitor(this ManyChannelMonitor chainListener,
            IDictionary<Primitives.LNOutPoint, uint256> oldBlockHashes, BlockHeader newBlockHeader, uint currentHeight,
            IBlockSource blockSource, Network n, ILogger? logger = null)
        {
            foreach (var kvp in oldBlockHashes)
            {
                await chainListener.SyncOneChannelListener(kvp.Key, kvp.Value, newBlockHeader, currentHeight, blockSource, n, logger);
            }
        }
    }
}