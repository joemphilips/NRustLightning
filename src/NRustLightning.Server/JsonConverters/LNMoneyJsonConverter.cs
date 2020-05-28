using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetLightning.Utils;

namespace NRustLightning.Server.JsonConverters
{
    public class LNMoneyJsonConverter : JsonConverter<LNMoney?>
    {
        public override LNMoney? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return LNMoney.MilliSatoshis(reader.GetInt64());
        }

        public override void Write(Utf8JsonWriter writer, LNMoney? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                return;
            }
            writer.WriteNumberValue(value.Value.MilliSatoshi);
        }
    }
}