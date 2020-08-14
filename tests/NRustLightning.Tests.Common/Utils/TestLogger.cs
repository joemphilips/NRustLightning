using System;
using System.Collections.Concurrent;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Common.Utils
{

    public class TestLogger : ILogger
    {
        private readonly ConsoleColor _color;
        public ConcurrentBag<string> Msgs = new ConcurrentBag<string>();
        
        public TestLogger(): this(ConsoleColor.White) {}

        public TestLogger(ConsoleColor color)
        {
            _color = color;
        }

        void ILogger.Log(FFILogLevel logLevel, string msg, string originalModulePath, string originalFileName, uint originalLineNumber)
        {
            Msgs.Add(msg);
            Console.ForegroundColor = _color;
            Console.WriteLine($"message received from ffi: \'{msg}\'");
        }
    }

}