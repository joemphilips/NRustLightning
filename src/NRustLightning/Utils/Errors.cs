using System.IO;
using NBitcoin;

namespace NRustLightning.Utils
{
    public static class Errors
    {
        public static void PubKeyNotCompressed(string argumentName, PubKey pk) =>
            throw new InvalidDataException($"invalid {argumentName}: public key must be compressed. {pk.ToHex()}");
        
        public static void InvalidDataLength(string argumentName, int actual, int expected) =>
            throw new InvalidDataException($"Invalid {argumentName}: length must be {expected}. it was {actual}");

        public static void AssertDataLength(string argumentName, int actual, int expected)
        {
            if (actual != expected)
                InvalidDataLength(argumentName, actual, expected);
        }
    }
}