using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly NBXplorerClientProvider _nbXplorerClientProvider;
        private readonly WalletService _walletService;
        private readonly RepositoryProvider _repositoryProvider;

        public WalletController(NRustLightningNetworkProvider networkProvider,
            NBXplorerClientProvider nbXplorerClientProvider, WalletService walletService,
            RepositoryProvider repositoryProvider)
        {
            _networkProvider = networkProvider;
            _nbXplorerClientProvider = nbXplorerClientProvider;
            _walletService = walletService;
            _repositoryProvider = repositoryProvider;
        }
        
        [HttpGet]
        [Route("{cryptoCode}")]
        public JsonResult GetWalletInfo(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var derivationStrategy = _walletService.GetOurDerivationStrategy(n);
            var resp = new WalletInfo {DerivationStrategy = derivationStrategy};
            return new JsonResult(resp, _repositoryProvider.GetSerializer(n).Options);
        }
        
        [HttpGet]
        [Route("{cryptoCode}/address")]
        public JsonResult Address(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode.ToLowerInvariant());
            var addr = _walletService.GetNewAddress(n);
            var resp = new GetNewAddressResponse { Address = addr };
            return new JsonResult(resp, _repositoryProvider.GetSerializer(n).Options);
        }
    }
}