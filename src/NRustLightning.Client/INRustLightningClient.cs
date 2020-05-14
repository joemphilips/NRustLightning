using System.Threading.Tasks;
using NRustLightning.Server.Models.Request;
using NRustLightning.Server.Models.Response;

namespace NRustLightning.Client
{
    public interface INRustLightningClient
    {
        Task<NodeInfo> GetInfoAsync();
        Task<bool> ConnectAsync(PeerConnectionString peerConnectionString);
    }
}