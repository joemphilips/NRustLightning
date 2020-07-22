using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using NBitcoin;

namespace NRustLightning.Server.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddCommandLineOptions(this IConfigurationBuilder config,
            ParseResult commandline)
        {
            if (commandline is null)
                throw new ArgumentNullException(nameof(commandline));

            var dict = new Dictionary<string, string>();
            foreach (var op in CommandLine.GetOptions())
            {
                if (op.Name == "nrustlightning")
                    continue;
                var s = op.Name.Replace(".", ":").Replace("_", "");
                var v = commandline.CommandResult.ValueForOption<object>(op.Name);
                if (v != null)
                {
                    dict.Add(s, v.ToString());
                }
            }
            return config.AddInMemoryCollection(dict);
        }
        
    }
}