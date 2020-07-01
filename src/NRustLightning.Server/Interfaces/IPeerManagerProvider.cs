using NRustLightning.Server.Networks;

namespace NRustLightning.Server.Interfaces
{
    public interface IPeerManagerProvider
    {
        PeerManager? TryGetPeerManager(string cryptoCode);
        PeerManager? TryGetPeerManager(NRustLightningNetwork network) => this.TryGetPeerManager(network.CryptoCode);
        PeerManager GetPeerManager(string cryptoCode) => TryGetPeerManager(cryptoCode) ?? Utils.Utils.Fail<PeerManager>($"Failed to get peer manager for cryptoCode: {cryptoCode}");
        PeerManager GetPeerManager(NRustLightningNetwork n) => this.GetPeerManager(n.CryptoCode);
    }
}