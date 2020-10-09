using System;
using System.Text;
using System.Text.RegularExpressions;
using DotNetLightning.Crypto;
using Microsoft.FSharp.Core;
using NBitcoin;
using NRustLightning.Infrastructure.Utils;

namespace NRustLightning.CLI
{
    public static class CommandLineUtils
    {
        private static Regex NumberDotsRegex = new Regex("[\\d.\\-\\n\\r\\t]*");
        private static Regex MultipleSpaces = new Regex(" [ ]+");

        public static bool TryReadAezeedFromTerminal(out ExtKey? rootKey)
        {
            rootKey = null;
            string? mnemonicStr = null;
            while (string.IsNullOrEmpty(mnemonicStr))
            {
                Console.WriteLine("Input your 24-word mnemonic separated by spaces: ");
                mnemonicStr = Console.ReadLine()?.Trim().ToLower();
            }

            // To allow the tool to also accept the copy/pasted version of the backup text (which contains numbers and
            // dts and multiple spaces), we do some more cleanup with regex.
            mnemonicStr =
                NumberDotsRegex.Replace(mnemonicStr, "");
            mnemonicStr = MultipleSpaces.Replace(mnemonicStr, " ").Trim();

            var mnemonics = new Mnemonic(mnemonicStr);
            Console.WriteLine();
            if (mnemonics.Words.Length != 24)
            {
                Console.WriteLine($"wrong cipher seed. mnemonic length: got {mnemonics.Words.Length}, expecting {24} words");
                return false;
            }
            var seedPassphrase = CliUtils.GetNewSeedPass();
            var r = mnemonics.ToCipherSeed(Encoding.UTF8.GetBytes(seedPassphrase), null);
            if (r.IsError)
            {
                Console.WriteLine(r.ErrorValue);
                return false;
            }
            var cipherSeed = r.ResultValue;
            rootKey = new ExtKey(cipherSeed.Entropy.AsSpan());
            return true;
        }
    }
}