using System;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models;

namespace NRustLightning.Infrastructure
{
    public class BitcoinRPCBlockSource : IBlockSource
    {
        private readonly RPCClient _client;

        public BitcoinRPCBlockSource(RPCClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        public async Task<BlockHeaderData> GetHeader(uint256 headerHash, uint? heightHint = null, CancellationToken ct = default)
        {
            BlockHeaderData headerData;
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

            return headerData;
        }

        public Task<Block> GetBlock(uint256 headerHash, CancellationToken ct = default)
        {
            return _client.GetBlockAsync(headerHash);
        }

        public async Task<Block> GetBestBlock(CancellationToken ct = default)
        {
            var hash =  await _client.GetBestBlockHashAsync();
            return await _client.GetBlockAsync(hash);
        }
    }
}