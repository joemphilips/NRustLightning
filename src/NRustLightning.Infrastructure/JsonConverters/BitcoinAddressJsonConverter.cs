using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBitcoin;

namespace NRustLightning.Infrastructure.JsonConverters
{
    public class BitcoinAddressJsonConverter : JsonConverter<BitcoinAddress>
    {
        private readonly Network _network;

        public BitcoinAddressJsonConverter(Network network)
        {
            _network = network ?? throw new ArgumentNullException(nameof(network));
        }
        public override BitcoinAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return BitcoinAddress.Create(s, _network);
        }

        public override void Write(Utf8JsonWriter writer, BitcoinAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}