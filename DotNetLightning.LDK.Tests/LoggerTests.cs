using System;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Interfaces;
using DotNetLightning.LDK.Tests.Utils;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class LoggerTests
    {
        internal class TestLogger : ILogger
        {

            private static Log log = (ref FFILogger logger, ref FFILogRecord record) =>
            {
                Console.WriteLine($"record {record.args}");
            };

            public ref Log Log => ref log;
        }
        
        // [Fact]
        [Fact(Skip = "make sure broadcaster works first")]
        public void LoggerTest()
        {
            var logger = Logger.Create(new TestLogger());
            logger.Test();
            logger.Dispose();
        }
    }
}