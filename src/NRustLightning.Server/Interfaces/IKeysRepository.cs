using DotNetLightning.Payment;
using NBitcoin;

namespace NRustLightning.Server.Interfaces
{
    public interface IKeysRepository
    {
        Key GetNodeSecret();
        PubKey GetNodeId();
    }
    
    public class KeysRepoMessageSigner : IMessageSigner
    {
        private readonly IKeysRepository _keysRepository;
        public KeysRepoMessageSigner(IKeysRepository keysRepository)
        {
            _keysRepository = keysRepository;
        }
        public byte[] SignMessage(uint256 obj0) =>
            _keysRepository.GetNodeSecret().SignCompact(obj0, false);
    }

    public static class IKeysRepositoryExtensoin
    {
        public static IMessageSigner AsMessageSigner(this IKeysRepository repo)
        {
            return new KeysRepoMessageSigner(repo);
        }
    }
}