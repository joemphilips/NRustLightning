using System;
using System.Collections.Concurrent;
using Microsoft.VisualBasic;
using NRustLightning.Adaptors;
using NRustLightning.Handles;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Utils
{

    internal class TestLogger : ILogger
    {
        public ConcurrentBag<string> Msgs = new ConcurrentBag<string>();

        private Log _log;
        public TestLogger()
        {
            _log = (ref FFILogRecord record) =>
            {
                Msgs.Add(record.Args);
                Console.WriteLine($"message received from ffi: \'{record.Args}\'");
            };
        }

        public Log Log => _log;
    }

}