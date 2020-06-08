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
        public LNMoney? Amount { get; set; } = null;

        [JsonConverter(typeof(uint256JsonConverter))]
        public uint256? PaymentSecret { get; set; } = null;

        public string Description { get; set; } = "no description";

        [JsonConverter(typeof(MMDDHHmmTimeSpanConverter))]
        public TimeSpan? Expiry { get; set; } = null;

        public string? FallbackAddress { get; set; } = null;

        public bool? EncodeDescriptionWithHash { get; set; } = false;
    }
}