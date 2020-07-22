using System;
using Microsoft.Extensions.Logging;
using NBitcoin.Logging;
using NRustLightning.Adaptors;
using NRustLightning.Infrastructure.Extensions;
using NRustLightning.Interfaces;
using ILogger = NRustLightning.Interfaces.ILogger;

namespace NRustLightning.Server.FFIProxies
{
    public class NativeLogger : ILogger
    {
        private readonly ILogger<NativeLogger> _logInternal;

        public NativeLogger(ILogger<NativeLogger> logInternal)
        {
            this._logInternal = logInternal;
        }
        void ILogger.Log(FFILogLevel logLevel, string msg, string originalModulePath, string originalFileName, uint originalLineNumber)
        {
            if (logLevel == FFILogLevel.Off) return;
            var msgFormatted =
                $"{msg}. module path: {originalModulePath}. file: {originalFileName}. line: {originalLineNumber}";
            _logInternal.Log(logLevel.AsLogLevel(), msgFormatted);
        }
    }
    
}