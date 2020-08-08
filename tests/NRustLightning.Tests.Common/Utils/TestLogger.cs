using System;
using System.Collections.Concurrent;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Common.Utils
{

    public class TestLogger : ILogger
    {
        public ConcurrentBag<string> Msgs = new ConcurrentBag<string>();

        void ILogger.Log(FFILogLevel logLevel, string msg, string originalModulePath, string originalFileName, uint originalLineNumber)
        {
            Msgs.Add(msg);
            Console.WriteLine($"message received from ffi: \'{msg}\'");
        }
    }

}