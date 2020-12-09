using System;
using System.Reflection;
using System.Text.Json;
using NBitcoin;

namespace NRustLightning.Infrastructure.JsonConverters.NBitcoinTypes
{
    public class NetworkJsonConverter : System.Text.Json.Serialization.JsonConverter<Network>
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Network).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override Network Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException();
            reader.AssertJsonType(JsonTokenType.String);
            var network = reader.GetString();
            if (network == null)
                throw new JsonException();
            if (network.Equals("MainNet", StringComparison.OrdinalIgnoreCase) || network.Equals("main", StringComparison.OrdinalIgnoreCase))
                return Network.Main;
            if (network.Equals("TestNet", StringComparison.OrdinalIgnoreCase) || network.Equals("test", StringComparison.OrdinalIgnoreCase))
                return Network.TestNet;
            if (network.Equals("RegTest", StringComparison.OrdinalIgnoreCase) || network.Equals("reg", StringComparison.OrdinalIgnoreCase))
                return Network.RegTest;
            var net = Network.GetNetwork(network);
            if (net != null)
                return net;
            throw new JsonException("Unknown network (valid values : main, test, reg)");
        }

        public override void Write(Utf8JsonWriter writer, Network value, JsonSerializerOptions options)
        {
            String str =
                (value == Network.Main) ?
                    "MainNet":
                (value == Network.TestNet) ?
                    "TestNet" :
                (value == Network.RegTest) ?
                    "RegTest" :
                   value.ToString();
            writer.WriteStringValue(str);
        }
    }
}