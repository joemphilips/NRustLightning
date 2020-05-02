using System;
using System.Threading.Tasks;
using NRustLightning.Server.Tests.Api;

namespace NRustLightning.Server.Tests.Support
{
    public sealed class TestCase
    {
        private readonly ITestCase _test;
        private readonly ServerProcess _server;
        private readonly Client _client;
        
        public TestCase(ITestCase test, ServerProcess server, Client client)
        {
            _test = test;
            _server = server;
            _client = client;
        }

        public string ServerOutput => _server.Output;
        public string Name => _test.GetType().FullName;

        public async Task Execute()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            if (_server.HasExited) throw new Exception("The server process has already exited");
            await _test.Execute(_client);
        }
    }
}