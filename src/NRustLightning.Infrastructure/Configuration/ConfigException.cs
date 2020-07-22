using System;

namespace NRustLightning.Infrastructure.Configuration
{
    public class ConfigException : Exception
    {
        public ConfigException() : base("") {}
        public ConfigException(string message) : base (message) {}
    }
}