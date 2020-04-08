using System;
using System.Collections.Concurrent;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Handles;
using DotNetLightning.LDK.Interfaces;
using Microsoft.VisualBasic;

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

    internal class TestLogger : ILogger
    {
        public ConcurrentBag<string> Msgs = new ConcurrentBag<string>();

        private Log _log;
        public TestLogger()
        {
            _log = (ref FFILogRecord record) =>
            {
                Msgs.Add(record.Args);
                Console.WriteLine($"message received from ffi is {record.Args}");
            };
        }

        public ref Log Log => ref _log;
    }

}