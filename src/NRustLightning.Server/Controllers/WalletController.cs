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
        private readonly RepositoryProvider _repositoryProvider;

        public WalletController(NRustLightningNetworkProvider networkProvider,
            INBXplorerClientProvider nbXplorerClientProvider, IWalletService walletService,
            RepositoryProvider repositoryProvider)
        {
            _networkProvider = networkProvider;
            _walletService = walletService;
            _repositoryProvider = repositoryProvider;
        }
        
        [HttpGet]
        [Route("{cryptoCode}")]
        public async Task<JsonResult> GetWalletInfo(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var derivationStrategy = _walletService.GetOurDerivationStrategy(n);
            var balance = await _walletService.GetBalanceAsync(n);
            var resp = new WalletInfo {DerivationStrategy = derivationStrategy, BalanceSatoshis = balance};
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