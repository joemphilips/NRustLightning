using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBitcoin;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public class uint256JsonConverter : JsonConverter<uint256?>
    {
        public override uint256? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            uint256 result;
            uint256.TryParse(reader.GetString(), out result);
            return result;
        }

        public override void Write(Utf8JsonWriter writer, uint256? value, JsonSerializerOptions options)
        {
            if (value is null)
                return;
            writer.WriteStringValue(value.ToString());
        }
    }
}