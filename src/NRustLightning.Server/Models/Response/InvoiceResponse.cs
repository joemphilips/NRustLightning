using DotNetLightning.Payment;
using Newtonsoft.Json;
using NRustLightning.Server.JsonConverters;

namespace NRustLightning.Server.Models.Response
{
    public class InvoiceResponse
    {
        [JsonConverter(typeof(PaymentRequestJsonConverter))]
        public PaymentRequest Invoice { get; set; } 
    }
}