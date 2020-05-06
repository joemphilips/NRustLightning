using System;
using System.Threading.Tasks;
using NRustLightning.Client;
using NRustLightning.Server.Tests.Api;

namespace NRustLightning.Server.Tests.Support
{
    public sealed class TestCase
    {
        private readonly ITestCase _test;
        private readonly DockerComposeProcess dockerCompose;
        private readonly NRustLightningClient _client;
        
        public TestCase(ITestCase test, DockerComposeProcess dockerCompose, NRustLightningClient client)
        {
            _test = test;
            this.dockerCompose = dockerCompose;
            _client = client;
        }

        public string ServerOutput => dockerCompose.Output;
        public string Name => _test.GetType().FullName;

        public async Task Execute()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            if (dockerCompose.HasExited) throw new Exception("The server process has already exited");
            await _test.Execute(_client);
        }
    }
}