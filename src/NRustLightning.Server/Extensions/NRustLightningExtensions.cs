using System;
using Microsoft.Extensions.Logging;
using NBitcoin;
using NRustLightning.Adaptors;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.Extensions
{
    public static class NRustLightningExtensions
    {
        public static LogLevel AsLogLevel(this FFILogLevel l) => l switch
        {
            FFILogLevel.Debug => LogLevel.Debug,
            FFILogLevel.Error => LogLevel.Error,
            FFILogLevel.Info => LogLevel.Information,
            FFILogLevel.Off => LogLevel.None,
            FFILogLevel.Trace => LogLevel.Trace,
            FFILogLevel.Warn => LogLevel.Warning,
            _ => throw new System.Exception("unreachable")
        };

    }
}