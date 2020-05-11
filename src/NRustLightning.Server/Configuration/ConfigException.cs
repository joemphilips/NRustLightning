using System;

namespace NRustLightning.Server.Configuration
{
    public class ConfigException : Exception
    {
        public ConfigException() : base("") {}
        public ConfigException(string message) : base (message) {}
    }
}