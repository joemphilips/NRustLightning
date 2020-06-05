using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    [Authorize(AuthenticationSchemes = "LSAT")]
    public class WalletController : ControllerBase
    {
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly NBXplorerClientProvider _nbXplorerClientProvider;
        private readonly WalletService _walletService;

        public WalletController(NRustLightningNetworkProvider networkProvider,
            NBXplorerClientProvider nbXplorerClientProvider, WalletService walletService)
        {
            _networkProvider = networkProvider;
            _nbXplorerClientProvider = nbXplorerClientProvider;
            _walletService = walletService;
        }
        [HttpGet]
        [Route("{cryptoCode}/address")]
        public GetNewAddressResponse Address(string cryptoCode)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode.ToLowerInvariant());
            var addr = _walletService.GetNewAddress(n);
            return new GetNewAddressResponse { Address = addr };
        }
    }
}