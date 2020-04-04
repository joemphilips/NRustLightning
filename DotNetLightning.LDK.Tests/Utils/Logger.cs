using System;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Interfaces;

namespace DotNetLightning.LDK.Tests.Utils
{
    public class Logger : IDisposable
    {
        private readonly LoggerHandle _handle;
        
        public Logger(LoggerHandle handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public static Logger Create(ILogger log)
        {
            Interop.create_logger(ref log.Log, out var handle);
            return new Logger(handle);
        }

        public void Test()
        {
            Interop.test_logger(_handle);
        }
        
        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}