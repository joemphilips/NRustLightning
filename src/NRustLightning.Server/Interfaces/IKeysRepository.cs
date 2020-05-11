using NBitcoin;

namespace NRustLightning.Server.Interfaces
{
    public interface IKeysRepository
    {
        Key GetNodeSecret();
        PubKey GetNodeId();
    }
}