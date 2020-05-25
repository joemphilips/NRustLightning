using System;
using System.Text.Json.Serialization;
using DotNetLightning.Utils;
using NBitcoin;
using NRustLightning.Server.JsonConverters;

namespace NRustLightning.Server.Models.Request
{
    public class InvoiceCreationOption
    {
        [JsonConverter(typeof(LNMoneyJsonConverter))]
        public LNMoney? Amount { get; set; }
        [JsonConverter(typeof(uint256JsonConverter))]
        public uint256? PaymentSecret { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(MMDDHHmmTimeSpanConverter))]
        public TimeSpan? Expiry { get; set; }
        public string? FallbackAddress { get; set; }

        public bool? EncodeDescriptionWithHash { get; set; } = false;
    }
}