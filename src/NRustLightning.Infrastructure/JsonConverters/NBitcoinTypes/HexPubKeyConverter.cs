using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBitcoin;

namespace NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes
{
    public class HexPubKeyConverter : JsonConverter<PubKey>
    {
        public override PubKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return new PubKey(s);
        }

        public override void Write(Utf8JsonWriter writer, PubKey value, JsonSerializerOptions options)
        {
            var s = value.ToHex();
            writer.WriteStringValue(s);
        }
    }
}