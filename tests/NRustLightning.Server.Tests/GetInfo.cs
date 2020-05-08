using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DockerComposeFixture;
using DockerComposeFixture.Compose;
using DockerComposeFixture.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace NRustLightning.Server.Tests
{
    public class GetInfo : IClassFixture<DockerFixture>
    {
        private readonly ITestOutputHelper output;

        public GetInfo(DockerFixture dockerFixture, ITestOutputHelper output)
        {
            this.output = output;
            dockerFixture.StartLNTestFixture(output, nameof(GetInfo));
        }

        [Fact]
        public void GetInfoTest()
        {
            
        }
    }
}