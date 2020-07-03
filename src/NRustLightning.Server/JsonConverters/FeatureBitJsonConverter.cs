using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetLightning.Serialize;
using DotNetLightning.Utils;
using Microsoft.AspNetCore.Routing;

namespace NRustLightning.Server.JsonConverters
{
    public class FeatureBitJsonConverter : JsonConverter<FeatureBit>
    {
        public override FeatureBit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FeatureBit result;
            if (reader.TokenType == JsonTokenType.Null)
                return null;
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
            result = FeatureBit.TryParse(bitArray).ResultValue;
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

        public override void Write(Utf8JsonWriter writer, FeatureBit value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("bitArray", value.ToString());
            writer.WriteString("hexBytes", value.ToByteArray().ToHexString());
            writer.WriteString("prettyPrint", value.PrettyPrint);
            writer.WriteEndObject();
        }
    }
}