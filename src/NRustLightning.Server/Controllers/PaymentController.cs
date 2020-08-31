using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNetLightning.Utils;
using DotNetLightning.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure.Interfaces;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Models.Response;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Infrastructure.Utils;
using NRustLightning.Server.Interfaces;
using NRustLightning.Server.Services;
using RustLightningTypes;

namespace NRustLightning.Server.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    // [Authorize(AuthenticationSchemes = "LSAT", Policy = "Readonly")]
    public class PaymentController : ControllerBase
    {
        private readonly NRustLightningNetworkProvider _networkProvider;
        private readonly RepositoryProvider _repositoryProvider;
        private readonly InvoiceService _invoiceService;

        public PaymentController(
            NRustLightningNetworkProvider networkProvider, RepositoryProvider repositoryProvider, InvoiceService invoiceService)
        {
            _networkProvider = networkProvider;
            _repositoryProvider = repositoryProvider;
            _invoiceService = invoiceService;
        }
        
        [HttpPost]
        [Route("{cryptoCode}/invoice")]
        public async Task<JsonResult> Invoice(string cryptoCode,[FromBody] InvoiceCreationOption option)
        {
            var n = _networkProvider.GetByCryptoCode(cryptoCode);
            var invoice = await _invoiceService.GetNewInvoice(n, option);
            var resp = new InvoiceResponse {Invoice = invoice};
            return new JsonResult(resp, _repositoryProvider.TryGetSerializer(n).Options);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // When Invoice is malformed, or there was no path
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // When payment failed
        [Route("pay/{bolt11Invoice}")]
        public async Task<IActionResult> Pay(string bolt11Invoice, long? amountMSat, CancellationToken ct)
        {
            var r = PaymentRequest.Parse(bolt11Invoice);
            if (r.IsError)
                return BadRequest($"Failed to parse invoice ({bolt11Invoice}): " + r.ErrorValue);
            
            var invoice = r.ResultValue;
            try
            {
                await _invoiceService.PayInvoice(invoice, amountMSat, ct);
            }
            catch (NRustLightningException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FFIException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}