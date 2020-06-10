using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBitcoin;
using NRustLightning.Server.JsonConverters;
using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Repository
{
    /// <summary>
    /// De/Serialize objects for persistence
    /// </summary>
    public class RepositorySerializer
    {
        private readonly NRustLightningNetwork _network;
        public Network Network => _network?.NBitcoinNetwork;
        
        public JsonSerializerOptions Options { get; } = new JsonSerializerOptions();

        public RepositorySerializer(NRustLightningNetwork network)
        {
            _network = network ?? throw new ArgumentNullException(nameof(network));
            ConfigureSerializer(Options);
        }

        public void ConfigureSerializer(JsonSerializerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Converters.Add(new DerivationStrategyJsonConverter(_network.NbXplorerNetwork.DerivationStrategyFactory));
            options.Converters.Add(new BitcoinAddressJsonConverter(_network.NBitcoinNetwork));
            options.Converters.Add(new PaymentRequestJsonConverter());
            // this is a last resort for serializing F# value, we usually want to try custom converters first.
            // So it must be specified at last.
            options.Converters.Add(new JsonFSharpConverter());
        }
    }
}