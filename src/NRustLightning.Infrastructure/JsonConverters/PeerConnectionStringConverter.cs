using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRustLightning.Infrastructure.Models.Request;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public class PeerConnectionStringConverter : JsonConverter<PeerConnectionString>
    {
        public override PeerConnectionString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (PeerConnectionString.TryCreate(reader.GetString(), out var result))
            {
                return result;
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, PeerConnectionString value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}