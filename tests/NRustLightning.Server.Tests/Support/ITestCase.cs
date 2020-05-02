using System.Threading.Tasks;
using NRustLightning.Server.Tests.Api;

namespace NRustLightning.Server.Tests.Support
{
    public interface ITestCase
    {
        Task Execute(Client client);
    }
}