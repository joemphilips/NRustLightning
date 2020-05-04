using Microsoft.Extensions.DependencyInjection;

namespace NRustLightning.Server.Tests.Support
{
    public static class Extensions
    {
        public static void AddDockerComposeService(this IServiceCollection service, string composeFilePath)
        {
            service.AddTransient<DataPath>();
            service.AddTransient<ListenUrl>();
            service.AddTransient(ctx => new DockerComposeProcess(composeFilePath, ctx.GetRequiredService<ListenUrl>(), ctx.GetRequiredService<DataPath>()));
        }
    }
}