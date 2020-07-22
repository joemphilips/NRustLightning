using DotNetLightning.Payment;
using Newtonsoft.Json;
using NRustLightning.Infrastructure.JsonConverters;

namespace NRustLightning.Infrastructure.Models.Response
{
    public class InvoiceResponse
    {
        [JsonConverter(typeof(PaymentRequestJsonConverter))]
        public PaymentRequest Invoice { get; set; } 
    }
}