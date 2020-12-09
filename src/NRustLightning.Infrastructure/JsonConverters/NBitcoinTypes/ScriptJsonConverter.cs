using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes
{
    public class ScriptJsonConverter : JsonConverter<Script>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Script).GetTypeInfo().IsAssignableFrom(typeToConvert.GetTypeInfo()) || typeof(WitScript).GetTypeInfo().IsAssignableFrom(typeToConvert.GetTypeInfo());
        }

        public override Script Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException();
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();
            try
            {
                if (typeToConvert == typeof(Script))
                    return Script.FromBytesUnsafe(Encoders.Hex.DecodeData((string)reader.GetString()));
            }
            catch (FormatException)
            {
            }
            throw new JsonException("A script should be a byte string");
        }

        public override void Write(Utf8JsonWriter writer, Script value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WriteStringValue(Encoders.Hex.EncodeData(((Script)value).ToBytes(false)));
            }
        }
    }
}