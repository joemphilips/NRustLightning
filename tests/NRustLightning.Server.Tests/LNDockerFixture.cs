using System;
using System.Threading.Tasks;
using DockerComposeFixture;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public class LNDockerFixture : DockerFixture, IAsyncLifetime
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly string _id;

        public LNDockerFixture(IMessageSink msg, ITestOutputHelper outputHelper) : base(msg)
        {
            _outputHelper = outputHelper;
            _id = Guid.NewGuid().ToString();
        }
        
        public async Task InitializeAsync()
        {
            await this.StartLNTestFixtureAsync(_outputHelper, _id);
        }

        public async Task DisposeAsync()
        {
            base.Dispose();
            return;
        }
    }
}