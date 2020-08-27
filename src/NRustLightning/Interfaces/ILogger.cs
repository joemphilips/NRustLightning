using System;
using System.Runtime.InteropServices;
using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{

    public interface ILogger
    {
        void Log(FFILogLevel logLevel, string msg, string originalModulePath, string originalFileName, uint originalLineNumber);
    }

    internal struct LoggerDelegatesHolder : IDisposable
    {
        private readonly ILogger _logger;

        private Log _log;
        private GCHandle _handle;
        private bool _disposed;

        public LoggerDelegatesHolder(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _log = (ref FFILogRecord record) =>
            {
                logger.Log(record.level, record.Args, record.ModulePath, record.File, record.line);
            };
            _handle = GCHandle.Alloc(_log);
            _disposed = false;
        }

        public Log Log => _log;

        public void Dispose()
        {
            if (_disposed) return;
            _handle.Free();
            _disposed = true;
        }
    }
}