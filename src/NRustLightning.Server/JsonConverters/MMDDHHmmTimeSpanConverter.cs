using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRustLightning.Server.JsonConverters
{
    public class MMDDHHmmTimeSpanConverter : JsonConverter<TimeSpan?>
    {
        private string _timespanFormatString = @"/MM/dd/hh/mm/ss";
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TimeSpan t;
            TimeSpan.TryParseExact(reader.GetString(), _timespanFormatString, null, out t);
            return t;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value is null)
                return;
            writer.WriteStringValue(value.Value.ToString(_timespanFormatString));
        }
    }
}