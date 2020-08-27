using System.Buffers;
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
            var balance = await _walletService.GetBalanceAsync(n);
            var outboundCaps = _peerManagerProvider.GetPeerManager(n).ChannelManager.ListChannels(_pool)
                .Select(c => c.OutboundCapacityMSat).ToArray();
            var offChainBalance = outboundCaps.Length == 0 ? 0 : outboundCaps.Aggregate((x, acc) => x + acc);
            var resp = new WalletInfo {DerivationStrategy = derivationStrategy, OnChainBalanceSatoshis = balance, OffChainBalanceMSat = offChainBalance};
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

        [HttpPost]
        [Route("{cryptoCode}/withdraw")]
        public async Task<ActionResult<uint256>> WithdrawFunds(string cryptoCode, WithdrawRequest req, CancellationToken ct)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var addr = BitcoinAddress.Create(req.DestinationAddress, n.NBitcoinNetwork);
            var tx = await _walletService.GetSendingTxAsync(addr, req.AmountSatoshi, n, ct);
            var cli = _clientProvider.GetClient(n);
            await cli.BroadcastAsync(tx, ct);
            return Ok(tx.GetHash());
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