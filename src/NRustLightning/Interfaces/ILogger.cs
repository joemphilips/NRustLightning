using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    public interface ILogger
    {
        Log Log { get; }
    }

    public abstract class Logger : ILogger
    {
        protected abstract void LogImpl(FFILogLevel logLevel, string msg, string originalModulePath, string originalFileName, uint originalLineNumber);

        protected virtual void LogCore(ref FFILogRecord record)
        {
            LogImpl(record.level, record.Args, record.ModulePath, record.File, record.line);
        }

        public Log Log => LogCore;
    }
}