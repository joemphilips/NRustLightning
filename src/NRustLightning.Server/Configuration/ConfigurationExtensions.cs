using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace NRustLightning.Server.Configuration
{
    public static class ConfigurationExtensions
    {
        public static T GetOrDefault<T>(this IConfiguration configuration, string key, T deafaultValue)
        {
            var str = configuration[key] ?? configuration[key.Replace(".", string.Empty)];
            if (str is null) return deafaultValue;
            if (typeof(T) == typeof(bool))
            {
                var trueValues = new[] { "1", "true" };
                var falseValues = new[] { "0", "false" };
                if (trueValues.Contains(str, StringComparer.OrdinalIgnoreCase))
                    return (T) (object) true;
                if (falseValues.Contains(str, StringComparer.OrdinalIgnoreCase))
                    return (T) (object) false;
                throw new FormatException();
            }
            if (typeof(T) == typeof(Uri))
                return (T) (object) new Uri(str, UriKind.Absolute);
            if (typeof(T) == typeof(string))
                return (T) (object) str;
            if (typeof(T) == typeof(int))
                return (T) (object) int.Parse(str, CultureInfo.InvariantCulture);

            throw new NotSupportedException("Configuration value does not support type " + typeof(T).Name);
        }

        public static IConfigurationBuilder AddCommandLineOptions(this IConfigurationBuilder config,
            ParseResult commandline)
        {
            if (commandline is null)
                throw new ArgumentNullException(nameof(commandline));

            var dict = new Dictionary<string, string>();
            foreach (var op in CommandLine.GetOptions())
            {
                var s = op.Name.Replace(".", ":");
                dict.Add(s, commandline.CommandResult.ValueForOption<string>(op.Name));
            }
            return config.AddInMemoryCollection(dict);
        }   
    }
}