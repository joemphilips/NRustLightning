using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetLightning.Serialization;
using DotNetLightning.Utils;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public class FeatureBitJsonConverter : JsonConverter<FeatureBits>
    {
        public override FeatureBits Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FeatureBits result;
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException();
            if (!reader.Read())
                throw new JsonException();
            
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            var bitArrayProp = reader.GetString();
            if (bitArrayProp != "bitArray")
                throw new JsonException();
            if (!reader.Read())
                throw new JsonException();
            var bitArray = reader.GetString();
            result = FeatureBits.TryParse(bitArray).ResultValue;
            if (!reader.Read())
                throw new JsonException();
            
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            if (reader.GetString() != "hexBytes")
                throw new JsonException();
            if (!(reader.Read() && reader.Read()))
                throw new JsonException();
            
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            if (reader.GetString() != "prettyPrint")
                throw new JsonException();
            if (!(reader.Read() && reader.Read()))
                throw new JsonException();

            return result;
        }

        public override void Write(Utf8JsonWriter writer, FeatureBits value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("bitArray", value.ToString());
            writer.WriteString("hexBytes", value.ToByteArray().ToHexString());
            writer.WriteString("prettyPrint", value.PrettyPrint);
            writer.WriteEndObject();
        }
    }
}