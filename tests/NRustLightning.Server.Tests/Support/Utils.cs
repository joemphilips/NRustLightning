using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NBitcoin;

namespace NRustLightning.Server.Tests.Support
{
    public static class Utils
    {
        private static void CheckConnection(int port)
        {
            var l = new TcpListener(IPAddress.Loopback, port);
            l.Start();
            l.Stop();
        }

        public static void FindEmptyPort(int[] ports)
        {
            var i = 0;
            while (i < ports.Length)
            {
                var port = RandomUtils.GetUInt32() % 4000;
                port = port + 10000;
                if (ports.Any(p => p == port))
                    continue;
                try
                {
                    CheckConnection((int)port);
                    ports[i] = (int) port;
                    i++;
                }
                catch (SocketException) {}
            }
        }

        public static async Task Retry(int times, TimeSpan delay, Func<Task<bool>> operationToRetryUntilItReturnsTrue)
        {
            var attempts = 0;
            while (!(await operationToRetryUntilItReturnsTrue.Invoke()))
            {
                attempts++;
                if (attempts == times)
                    throw new Exception("operation failed.");
                await Task.Delay(delay);
            }
        }
    }
}