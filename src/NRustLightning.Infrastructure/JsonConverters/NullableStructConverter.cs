using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public class NullableStructConverter<T> : JsonConverter<T?> where T : struct
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value == null) writer.WriteNullValue();
            else
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
    
    public class NullableStructConverterFactory : JsonConverterFactory {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            if (!typeToConvert.IsGenericType || typeToConvert.GetGenericTypeDefinition() != typeof(System.Nullable<>))
                return false;

            var structType = typeToConvert.GenericTypeArguments[0];
            if (structType.IsPrimitive || structType.Namespace != null && structType.Namespace.StartsWith(nameof(System)) || structType.IsEnum)
                return false;
            return true;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var structType = typeToConvert.GenericTypeArguments[0];
            return (JsonConverter) Activator.CreateInstance(
                typeof(NullableStructConverter<>).MakeGenericType(structType));
        }
    }
}