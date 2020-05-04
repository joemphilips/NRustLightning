using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NRustLightning.Server.Tests.Api;

namespace NRustLightning.Server.Tests.Support
{
    public class TestCaseRunnerModule : Module
    {
            private readonly string _composeFilePath;
    
            public TestCaseRunnerModule(string composeFilePath)
            {
                _composeFilePath = composeFilePath;
            }
    
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterAssemblyTypes(ThisAssembly)
                    .As<ITestCase>();
    
                builder.RegisterType<TestCaseRunner>();
    
                builder.RegisterType<DataPath>().InstancePerOwned<TestCase>();
                builder.RegisterType<ListenUrl>().InstancePerOwned<TestCase>();
    
                builder.Register(ctx =>
                        new DockerComposeProcess(_composeFilePath, ctx.Resolve<ListenUrl>(), ctx.Resolve<DataPath>()))
                    .As<DockerComposeProcess>()
                    .InstancePerOwned<TestCase>();
    
                builder.Register(ctx => new Client(ctx.Resolve<ListenUrl>()))
                    .As<Client>()
                    .InstancePerOwned<TestCase>();
    
                builder.RegisterAdapter<ITestCase, TestCase>((ctx, test) =>
                    new TestCase(test, ctx.Resolve<DockerComposeProcess>(), ctx.Resolve<Client>()));
        }
    }
}