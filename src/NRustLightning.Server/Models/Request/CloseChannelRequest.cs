using NBitcoin;
using Newtonsoft.Json;
using NRustLightning.Server.JsonConverters;

namespace NRustLightning.Server.Models.Request
{
    public class CloseChannelRequest
    {
        [JsonConverter(typeof(HexPubKeyConverter))]
        public PubKey TheirNetworkKey { get; set; }
    }
}