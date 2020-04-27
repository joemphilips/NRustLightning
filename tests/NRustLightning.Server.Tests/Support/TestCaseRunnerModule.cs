using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NRustLightning.Server.Tests.Api;

namespace NRustLightning.Server.Tests.Support
{
    internal static class Extension
    {
        public static void AddTestCaseRunnerModule(this IServiceCollection service, string binPath)
        {
            service.AddSingleton<TestCaseRunner>();
            service.AddTransient<DataPath>();
            service.AddTransient<ListenUrl>();
            service.TryAddTransient(x => new ServerProcess(binPath, x.GetRequiredService<ListenUrl>(), x.GetRequiredService<DataPath>()));
            service.TryAddTransient(ctx => new Client(ctx.GetRequiredService<ListenUrl>()));
            service.TryAddEnumerable(ServiceDescriptor.Transient<ITestCase, GetInfo>());
        }
    }
}