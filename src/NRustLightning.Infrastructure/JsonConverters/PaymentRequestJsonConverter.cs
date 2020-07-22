using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetLightning.Payment;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public class PaymentRequestJsonConverter: JsonConverter<PaymentRequest>
    {
        public override PaymentRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            var r = PaymentRequest.Parse(s);
            if (r.IsOk)
                return r.ResultValue;
            throw new JsonException(r.ErrorValue);
        }

        public override void Write(Utf8JsonWriter writer, PaymentRequest value, JsonSerializerOptions options)
        {
            if (value is null)
                return;
            var s = value.ToString();
            writer.WriteStringValue(s);
        }
    }
}