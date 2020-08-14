using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models;

namespace NRustLightning.Infrastructure
{
    public class BitcoinRPCBlockSource : IBlockSource, IDisposable
    {
        private readonly RPCClient _client;

        public BitcoinRPCBlockSource(RPCClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        
        private ConcurrentDictionary<uint256, BlockHeaderData> _headerCache = new ConcurrentDictionary<uint256,BlockHeaderData>();
        private ConcurrentDictionary<uint256, Block> _blockCache = new ConcurrentDictionary<uint256, Block>();
        public async Task<BlockHeaderData> GetHeader(uint256 headerHash, uint? heightHint = null, CancellationToken ct = default)
        {
            if (_headerCache.TryGetValue(headerHash, out var headerData))
                return headerData;
            if (heightHint != null)
            {
                var header = await _client.GetBlockHeaderAsync(headerHash);
                headerData = new BlockHeaderData(heightHint.Value, header);
            }
            else
            {
                var blockInfo = await _client.GetBlockAsync(headerHash, GetBlockVerbosity.WithOnlyTxId);
                headerData = new BlockHeaderData((uint)blockInfo.Height, blockInfo.Header);
            }

            _headerCache.TryAdd(headerHash, headerData);
            return headerData;
        }

        public async Task<Block> GetBlock(uint256 headerHash, CancellationToken ct = default)
        {
            if (_blockCache.TryGetValue(headerHash, out var block))
                return block;
            block = await _client.GetBlockAsync(headerHash);
            _blockCache.TryAdd(headerHash, block);
            return block;
        }

        public async Task<Block> GetBestBlock(CancellationToken ct = default)
        {
            var hash =  await _client.GetBestBlockHashAsync();
            return await _client.GetBlockAsync(hash);
        }

        public void Dispose()
        {
            _blockCache.Clear();
            _headerCache.Clear();
        }
    }
}