using System;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Interfaces;
using DotNetLightning.LDK.Tests.Utils;
using Xunit;

namespace DotNetLightning.LDK.Tests
{
    public class LoggerTests
    {
        [Fact]
        public void LoggerTest()
        {
            var l = new TestLogger();
            var logger = Logger.Create(l);
            logger.Test();
            Assert.Single(l.Msgs);
            Assert.All(l.Msgs, item => Assert.Equal("warn_msg", item));
            logger.Dispose();
        }
    }
}