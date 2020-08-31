using NBitcoin;
using Newtonsoft.Json;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure.JsonConverters;
using NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes;

namespace NRustLightning.Infrastructure.Models.Request
{
    public class OpenChannelRequest
    {
        [JsonConverter(typeof(HexPubKeyConverter))]
        public PubKey TheirNetworkKey { get; set; }
        public ulong ChannelValueSatoshis { get; set; }
        public ulong PushMSat { get; set; }
        
        [JsonConverter(typeof(NullableStructConverter<UserConfig>))]
        public UserConfig? OverrideConfig { get; set; } = null;
    }
}