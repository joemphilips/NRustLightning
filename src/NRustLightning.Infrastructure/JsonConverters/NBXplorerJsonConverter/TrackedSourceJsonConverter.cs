using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBXplorer;
using NBXplorer.Models;

namespace NRustLightning.Infrastructure.JsonConverters.NBXplorerJsonConverter
{
    public class TrackedSourceJsonConverter : JsonConverter<TrackedSource>
    {
        public NBXplorerNetwork Network { get; }
        public TrackedSourceJsonConverter(NBXplorerNetwork network)
        {
            Network = network;
        }
        public override TrackedSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                return null;

            var s = reader.GetString();
            if (TrackedSource.TryParse(s, out var v, Network))
                return v;
            throw new JsonException($"Invalid TrackedSource {s}");
        }

        public override void Write(Utf8JsonWriter writer, TrackedSource value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}