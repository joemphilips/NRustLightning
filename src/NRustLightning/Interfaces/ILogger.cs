using System;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{

    public interface ILogger
    {
        void Log(FFILogLevel logLevel, string msg, string originalModulePath, string originalFileName, uint originalLineNumber);
    }

    internal struct LoggerDelegatesHolder
    {
        private readonly ILogger _logger;

        private Log _log;
        public LoggerDelegatesHolder(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _log = (ref FFILogRecord record) =>
            {
                logger.Log(record.level, record.Args, record.ModulePath, record.File, record.line);
            };
        }

        public Log Log => _log;
    }
}