using NBitcoin;

namespace Macaroons
{
    public class NBitcoinCryptoAlgorithm: CryptoAlgorithm
    {
        Key k = new Key();
        public override byte[] Encrypt(byte[] key, byte[] plainText)
        {
            return k.PubKey.Encrypt(plainText);
        }

        public override byte[] Decrypt(byte[] key, byte[] cipherText)
        {
            return k.Decrypt(cipherText);
        }
    }
}