using System.Security.Cryptography;
using NBitcoin;

namespace Macaroons
{
    public class SecretBoxCryptoAlgorithm : CryptoAlgorithm
    {
        protected bool UseRandomNonce { get; } = false;
        public const int SECRET_BOX_ZERO_BYTES = 16;
        public const int SECRET_BOX_NONCE_BYTES = 24;
        
        public SecretBoxCryptoAlgorithm() {}

        public SecretBoxCryptoAlgorithm(bool useRandomNonce)
        {
            UseRandomNonce = useRandomNonce;
        }
        
        public override byte[] Encrypt(byte[] key, byte[] plainText)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Decrypt(byte[] key, byte[] cipherText)
        {
            throw new System.NotImplementedException();
        }
    }
}