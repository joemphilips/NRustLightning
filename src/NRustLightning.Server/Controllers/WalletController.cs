using System.Buffers;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using NRustLightning.Infrastructure.Models;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Models.Response;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    // [Authorize(AuthenticationSchemes = "LSAT", Policy = "Readonly")]
    public class WalletController : ControllerBase
    {
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly IWalletService _walletService;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly RepositoryProvider _repositoryProvider;
        private readonly INBXplorerClientProvider _clientProvider;
        private readonly MemoryPool<byte> _pool;

        public WalletController(NRustLightningNetworkProvider networkProvider,
            IWalletService walletService,
            PeerManagerProvider peerManagerProvider,
            RepositoryProvider repositoryProvider,
            INBXplorerClientProvider clientProvider)
        {
            _networkProvider = networkProvider;
            _walletService = walletService;
            _peerManagerProvider = peerManagerProvider;
            _repositoryProvider = repositoryProvider;
            _clientProvider = clientProvider;
            _pool = MemoryPool<byte>.Shared;
        }
        
        [HttpGet]
        [Route("{cryptoCode}")]
        public async Task<JsonResult> GetWalletInfo(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var derivationStrategy = await _walletService.GetOurDerivationStrategyAsync(n);
            var onChainBalance = await _walletService.GetBalanceAsync(n);
            var offChainBalance = _peerManagerProvider.GetPeerManager(n).ChannelManager.ListChannels(_pool)
                .Select(c => c.OutboundCapacityMSat + c.InboundCapacityMSat)
                .DefaultIfEmpty()
                .Aggregate((x, acc) => x + acc);
            var resp = new WalletInfo {DerivationStrategy = derivationStrategy, OnChainBalanceSatoshis = onChainBalance, OffChainBalanceMSat = offChainBalance};
            return new JsonResult(resp, _repositoryProvider.TryGetSerializer(n).Options);
        }
        
        [HttpGet]
        [Route("{cryptoCode}/address")]
        public async Task<JsonResult> Address(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var addr = await _walletService.GetNewAddressAsync(n);
            var resp = new GetNewAddressResponse { Address = addr };
            return new JsonResult(resp, _repositoryProvider.TryGetSerializer(n).Options);
        }

        /// <summary>
        /// Withdraw on-chain funds to the user specified address.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST /v1/wallet/btc/withdraw
        ///     {
        ///         "AmountSatoshi": 5000,
        ///         "DestinationAddress": "2NCHX7nn2zxNsgjqq4RqcxJ6iYH5kSuZz91"
        ///     }
        /// </remarks>
        /// <param name="cryptoCode">cryptoCode (e.g. "btc")</param>
        /// <param name="req">Details for the tx to create. Specify `0` to AmountSatoshis if you want to sweep all your funds.</param>
        /// <param name="ct"></param>
        /// <returns>Transaction id of the sending tx</returns>
        /// <response code="201"> Returns tx id of the sending tx</response>
        [HttpPost]
        [Route("{cryptoCode}/withdraw")]
        public async Task<ActionResult<uint256>> WithdrawFunds(string cryptoCode, WithdrawRequest req, CancellationToken ct)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var addr = BitcoinAddress.Create(req.DestinationAddress, n.NBitcoinNetwork);
            var tx = await _walletService.GetSendingTxAsync(addr, req.AmountSatoshi, n, ct);
            var cli = _clientProvider.GetClient(n);
            await cli.BroadcastAsync(tx, ct);
            return CreatedAtRoute($"{cryptoCode}/withdraw",tx.GetHash());
        }

        [HttpGet]
        [Route("{cryptoCode}/utxos")]
        public async Task<JsonResult> ListUnspent(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var utxos = await _walletService.ListUnspent(n);
            var ser = _repositoryProvider.GetSerializer(n);
            return new JsonResult(utxos, ser.Options);
        }
    }
}