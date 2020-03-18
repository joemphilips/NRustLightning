using System;
using System.Text;
using System.Transactions;
using Xunit;
using DotNetLightning.LDK;

namespace DotNetLightning.LDK.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var chacha20 = new ChaCha20Poly1305();

            var myString = new byte[7] {2, 2, 2, 3, 3, 3, 4};// ASCIIEncoding.ASCII.GetBytes("foobar").AsSpan();
            var encrypted = chacha20.Encrypt(myString.AsSpan<byte>());
            Console.WriteLine(encrypted);
        }
    }
}