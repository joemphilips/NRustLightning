using System;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Tests.Utils;
using Xunit;

namespace NRustLightning.Tests
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