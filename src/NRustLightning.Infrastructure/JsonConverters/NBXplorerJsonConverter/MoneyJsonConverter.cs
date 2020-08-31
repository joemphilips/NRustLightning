using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBitcoin;

namespace NRustLightning.Infrastructure.JsonConverters.NBXplorerJsonConverter
{
    public class MoneyJsonConverter : JsonConverter<IMoney>
    {
        public override IMoney Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IMoney value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}