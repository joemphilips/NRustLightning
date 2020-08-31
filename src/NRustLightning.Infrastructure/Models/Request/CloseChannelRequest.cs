using NBitcoin;
using Newtonsoft.Json;
using NRustLightning.Infrastructure.JsonConverters;
using NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes;

namespace NRustLightning.Infrastructure.Models.Request
{
    public class CloseChannelRequest
    {
        [JsonConverter(typeof(HexPubKeyConverter))]
        public PubKey TheirNetworkKey { get; set; }
    }
}