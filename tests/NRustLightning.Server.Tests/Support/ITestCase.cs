using System.Threading.Tasks;
using NRustLightning.Client;
using NRustLightning.Server.Tests.Api;

namespace NRustLightning.Server.Tests.Support
{
    public interface ITestCase
    {
        Task Execute(NRustLightningClient client);
    }
}