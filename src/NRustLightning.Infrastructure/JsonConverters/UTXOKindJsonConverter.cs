using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NRustLightning.Infrastructure.Models.Response;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public class UTXOKindJsonConverter : JsonConverter<UTXOKind>
    {
        public override UTXOKind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Enum.Parse<UTXOKind>(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, UTXOKind value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToFormatString());
        }
    }
}