using System;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Interfaces;
using DotNetLightning.LDK.Tests.Utils;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class LoggerTests
    {
        [Fact(Skip = "it crashes after calling logger.log in test_logger")]
        public void LoggerTest()
        {
            var l = new TestLogger();
            var logger = Logger.Create(l);
            logger.Test();
            logger.Dispose();
        }
    }
}