using System.Collections;
using System.Text.Json;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public static class JsonConverterUtils
    {
        public static string ReadStringValueForProperty(this Utf8JsonReader reader, string expectedPropertyName)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            var prop = reader.GetString();
            if (prop != expectedPropertyName)
                throw new JsonException();
            if (!reader.Read())
                throw new JsonException();
            var r = reader.GetString();
            reader.Read();
            return r;
        }
        public static int ReadIntValueForProperty(this Utf8JsonReader reader, string expectedPropertyName)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();
            var prop = reader.GetString();
            if (prop != expectedPropertyName)
                throw new JsonException();
            if (!reader.Read())
                throw new JsonException();
            var r = reader.GetInt32();
            reader.Read();
            return r;
        }
        
        public static void AssertJsonType(this Utf8JsonReader reader, JsonTokenType expectedType)
        {
            if (reader.TokenType != expectedType)
                throw new JsonException($"Unexpected json token type, expected is {expectedType} and actual is {reader.TokenType}");
        }
        public static void AssertJsonType(this Utf8JsonReader reader, JsonTokenType[] anyExpectedTypes)
        {
            if (!((IList) anyExpectedTypes).Contains(reader.TokenType))
                throw new JsonException($"Unexpected json token type, expected are {string.Join(", ", anyExpectedTypes)} and actual is {reader.TokenType}");
        }
    }
}