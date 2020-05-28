namespace Macaroons
{
    public abstract class CryptoAlgorithm
    {
        public abstract byte[] Encrypt(byte[] key, byte[] plainText);
        public abstract byte[] Decrypt(byte[] key, byte[] cipherText);
    }
}