using System;
using DotNetLightning.Crypto;
using NRustLightning.Infrastructure.Configuration;

namespace NRustLightning.Infrastructure.Utils
{
    public static class CliUtils
    {
        public static bool AskUserYesOrNo(bool? defaultAnswer = null)
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (input.IsYes())
                    return true;
                if (input.IsNo())
                    return false;
                if (defaultAnswer != null)
                    return defaultAnswer.Value;
                Console.WriteLine("Please answer by yes or no");
            }
        }
        
        public static string GetNewSeedPass()
        {
            string? pin;
            Console.WriteLine("========================================================================");
            Console.WriteLine("Please enter the cipher seed passphrase to secure your seed on disk ");
            Console.WriteLine("It is recommended to be longer than 10 characters to be cryptographically secure");
            Console.WriteLine("Please do not forget this cipher seed passphrase. You will need it when restarting" +
                              "the server");
            Console.WriteLine("========================================================================");
            while (true)
            {
                Console.WriteLine("Please enter your cipher seed passphrase: ");
                var pin1 = Console.ReadLine();
                if (string.IsNullOrEmpty(pin1?.Trim()))
                {
                    Console.WriteLine($"Are you sure you want to use empty pin code? [y/N]");
                    var isYes = CliUtils.AskUserYesOrNo(false);
                    if (isYes)
                        return "";
                    else
                        continue;
                }
                Console.WriteLine("Please enter again:");
                var pin2 = Console.ReadLine();
                if (pin1 == pin2)
                {
                    pin = pin1;
                    break;
                }
                else
                {
                    Console.WriteLine("cipher seed passphrase mismatch! try again");
                }
            }
            return pin;
        }

        public static string GetMnemonic()
        {
        }

        public static string GetSeedPass(byte[] encipheredCipherSeed)
        {
            if (encipheredCipherSeed.Length != 33)
                throw new ConfigException($"{nameof(encipheredCipherSeed)} must be length 33! It was {encipheredCipherSeed.Length}");
            string? seedPass;
            bool isValid;
            while (true)
            {
                Console.WriteLine("Please enter your cipher seed passphrase: ");
                var pin1 = Console.ReadLine();
                Console.WriteLine("Please enter again:");
                var pin2 = Console.ReadLine();
                if (pin1 == pin2)
                {
                    seedPass = pin1;
                    break;
                }
                Console.WriteLine("cipher seed passphrase mismatch! try again");
            }
            return seedPass;
        }
    }
}