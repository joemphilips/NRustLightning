using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRustLightning.Server.Tests.Support;
using Xunit;

namespace NRustLightning.Server.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var hostBuilder = new HostBuilder().ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseEnvironment("Test");
                webHost.Configure(async ctx => {});
            });

            var host = await hostBuilder.StartAsync();
        }

        [Fact]
        public void RunRegisteredTests()
        {
            var service = new ServiceCollection();
            service.AddTestCaseRunnerModule("tmp");
        }
    }
}
