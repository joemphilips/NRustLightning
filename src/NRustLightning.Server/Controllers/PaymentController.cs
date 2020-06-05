using System.Net.Http;
using System.Threading.Tasks;
using DotNetLightning.Utils;
using DotNetLightning.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;
using NRustLightning.Server.Networks;
using NRustLightning.Server.Services;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    [Authorize(AuthenticationSchemes = "LSAT")]
    public class PaymentController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly PeerManagerProvider _peerManagerProvider;
        private readonly NRustLightningNetworkProvider _networkProvider;

        public PaymentController(IInvoiceRepository invoiceRepository, PeerManagerProvider peerManagerProvider,
            NRustLightningNetworkProvider networkProvider)
        {
            _invoiceRepository = invoiceRepository;
            _peerManagerProvider = peerManagerProvider;
            _networkProvider = networkProvider;
        }
        
        [HttpGet]
        [Route("{cryptoCode}/invoice")]
        public InvoiceResponse Invoice(string cryptoCode,[FromBody] InvoiceCreationOption option)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var preimage = Primitives.PaymentPreimage.Create(RandomUtils.GetBytes(32));
            var invoice = _invoiceRepository.GetNewInvoice(n, preimage, option);
            return new InvoiceResponse {Invoice = invoice};
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // When Invoice is malformed, or there was no path
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // When payment failed
        public async Task<IActionResult> Pay(string cryptoCode, string bolt11Invoice)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var r = PaymentRequest.Parse(bolt11Invoice);
            if (r.IsOk)
            {
                await _invoiceRepository.PaymentStarted(r.ResultValue);
                return Ok();
            }
            throw new HttpRequestException(r.ErrorValue);
        }
    }
}