using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetLightning.Payment;
using NBitcoin;
using NRustLightning.Infrastructure.Repository;
using NRustLightning.Interfaces;
using RustLightningTypes;

namespace NRustLightning.Infrastructure.Interfaces
{
    public interface IKeysRepository : IKeysInterface
    {
        PubKey GetNodeId() => this.GetNodeSecret().PubKey;

        Key GetDestinationKey();

        RepositorySerializer Serializer { get; set; }

        ChannelKeys DeriveChannelKeys(ulong amountSat, ulong param1, ulong param2);
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

    public static class IKeysRepositoryExtension
    {
        public static IMessageSigner AsMessageSigner(this IKeysRepository repo)
        {
            return new KeysRepoMessageSigner(repo);
        }
        public static Script GetRevokeableRedeemScript(this IKeysRepository _, PubKey revocationPubKey, ushort toSelfDelay, PubKey localDelayedPaymentPubkey)
        {
            var opList = new List<Op>();
            opList.Add(OpcodeType.OP_IF);
            opList.Add(Op.GetPushOp(revocationPubKey.ToBytes()));
            opList.Add(OpcodeType.OP_ELSE);
            opList.Add(Op.GetPushOp((long)toSelfDelay));
            opList.Add(OpcodeType.OP_CHECKSEQUENCEVERIFY);
            opList.Add(OpcodeType.OP_DROP);
            opList.Add(Op.GetPushOp(localDelayedPaymentPubkey.ToBytes()));
            opList.Add(OpcodeType.OP_ENDIF);
            opList.Add(OpcodeType.OP_CHECKSIG);
            return new Script(opList);
        }

    }
}