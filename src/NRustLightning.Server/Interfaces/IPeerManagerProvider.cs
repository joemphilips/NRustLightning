using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface IPeerManagerProvider
    {
        PeerManager? GetPeerManager(string cryptoCode);
        PeerManager? GetPeerManager(NRustLightningNetwork network) => this.GetPeerManager(network.CryptoCode);
    }
}