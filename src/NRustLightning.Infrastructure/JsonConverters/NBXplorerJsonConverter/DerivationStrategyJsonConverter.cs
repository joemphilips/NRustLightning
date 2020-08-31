using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBXplorer.DerivationStrategy;

namespace NRustLightning.Infrastructure.JsonConverters.NBXplorerJsonConverter
{
    public class DerivationStrategyJsonConverter : JsonConverter<DerivationStrategyBase>
    {
        public DerivationStrategyFactory? Factory;

        public DerivationStrategyJsonConverter(DerivationStrategyFactory factory)
        {
            Factory = factory;
        }
        public override DerivationStrategyBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType ==  JsonTokenType.Null)
                return null;

            try
            {
                var result = Factory?.Parse(reader.GetString());
                if (result == null)
                {
                    throw new JsonException("invalid derivation strategy");
                }

                if (!typeToConvert.GetTypeInfo().IsAssignableFrom(result.GetType().GetTypeInfo()))
                {
                    throw new JsonException(
                        $"Invalid derivation strategy expected {typeToConvert.Name}, actual {result.GetType().Name}");
                }
                return result;
            }
            catch (FormatException ex)
            {
                throw new JsonException($"Invalid derivation strategy ", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, DerivationStrategyBase value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}