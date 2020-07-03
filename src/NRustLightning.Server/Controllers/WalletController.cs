using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Repository;
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
        private readonly IPeerManagerProvider _peerManagerProvider;
        private readonly RepositoryProvider _repositoryProvider;
        private readonly MemoryPool<byte> _pool;

        public WalletController(NRustLightningNetworkProvider networkProvider,
            IWalletService walletService,
            IPeerManagerProvider peerManagerProvider,
            RepositoryProvider repositoryProvider)
        {
            _networkProvider = networkProvider;
            _walletService = walletService;
            _peerManagerProvider = peerManagerProvider;
            _repositoryProvider = repositoryProvider;
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
            return new JsonResult(resp, _repositoryProvider.GetSerializer(n).Options);
        }
        
        [HttpGet]
        [Route("{cryptoCode}/address")]
        public async Task<JsonResult> Address(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode.ToLowerInvariant());
            var addr = await _walletService.GetNewAddressAsync(n);
            var resp = new GetNewAddressResponse { Address = addr };
            return new JsonResult(resp, _repositoryProvider.GetSerializer(n).Options);
        }
    }
}