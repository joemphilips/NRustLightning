using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes
{
    public class DateTimeToUnixTimeConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.AssertJsonType(JsonTokenType.Number);

            var result = NBitcoin.Utils.UnixTimeToDateTime(reader.GetUInt64());
            return result;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            var epoch = NBitcoin.Utils.UnixTimeToDateTime(0);
            if (value < epoch)
                value = epoch;
            writer.WriteNumberValue(NBitcoin.Utils.DateTimeToUnixTime(value));
        }
    }
}