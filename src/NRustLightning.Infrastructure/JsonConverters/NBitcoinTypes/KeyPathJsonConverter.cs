using System;
using System.Reflection;
using System.Text.Json;
using NBitcoin;

namespace NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes
{
    public class KeyPathJsonConverter : System.Text.Json.Serialization.JsonConverter<KeyPath>
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(KeyPath).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override KeyPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException();
            reader.AssertJsonType(JsonTokenType.String);
            if (KeyPath.TryParse(reader.GetString(), out var k) && k != null)
                return k;
            throw new JsonException("Invalid key path");
        }

        public override void Write(Utf8JsonWriter writer, KeyPath value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}