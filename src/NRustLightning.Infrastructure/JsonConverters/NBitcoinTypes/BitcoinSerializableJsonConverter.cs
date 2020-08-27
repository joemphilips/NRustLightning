using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.JsonConverters;

namespace NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes
{
    public class BitcoinSerializableJsonConverter<T> : JsonConverter<T> where T : IBitcoinSerializable, new()
    {
        public Network Network { get; }
        public BitcoinSerializableJsonConverter(Network network)
        {
            Network = network;
        }
        public override T Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
        {
            reader.AssertJsonType(JsonTokenType.String);
            try
            {
                IBitcoinSerializable obj = null;
                var bytes = Encoders.Hex.DecodeData(reader.GetString());
                if (!Network.Consensus.ConsensusFactory.TryCreateNew(objectType, out obj))
                {
                    if (objectType == typeof(PubKey))
                    {
                        obj = new PubKey(bytes);
                    }
                    else
                    {
                        obj = (T)Activator.CreateInstance(objectType);
                    }
                }
                obj.ReadWrite(bytes, Network);
                return (T)obj;
            }
            catch (EndOfStreamException)
            {
            }
            catch (FormatException)
            {
            }
            throw new JsonException("Invalid bitcoin object of type " + objectType.Name);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var bytes = ((IBitcoinSerializable)value).ToBytes();
            writer.WriteStringValue(Encoders.Hex.EncodeData(bytes));
        }
    }
    
    public class BitcoinSerializableJsonConverterFactory : JsonConverterFactory
    {
        public Network Network { get; }
        public BitcoinSerializableJsonConverterFactory(Network network)
        {
            Network = network;
        }
        public override bool CanConvert(Type typeToConvert)
        {
            return
                typeof(IBitcoinSerializable).GetTypeInfo().IsAssignableFrom(typeToConvert.GetTypeInfo());
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {

            JsonConverter converter = (JsonConverter) Activator.CreateInstance(
                typeof(BitcoinSerializableJsonConverter<>).MakeGenericType(typeToConvert),
                new object [] {Network});
            return converter;
        }
    }
}