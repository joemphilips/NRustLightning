using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using NRustLightning.Server.Tests.Api;

namespace NRustLightning.Server.Tests.Support
{
    public class DockerComposeProcess : IDisposable
    {
        private readonly ManualResetEvent _errorComplete = new ManualResetEvent(false);
        private readonly StringWriter _output = new StringWriter();
        private readonly ManualResetEvent _outputComplete = new ManualResetEvent(false);
        private readonly Process _process;
        
        private readonly object _sync = new object();
        public Client Client;

        public DockerComposeProcess(string composeFilePath, string listenUrl, string dataPath)
        {
            Client = new Client(listenUrl);
            var startInfo = new ProcessStartInfo()
            {
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = "docker-compose",
                Arguments = $"-f {composeFilePath} up",
            };
            startInfo.EnvironmentVariables["LISTEN_URL"] = listenUrl;
            startInfo.EnvironmentVariables["DATA_PATH"] = dataPath;
            startInfo.EnvironmentVariables["BITCOIND_RPC_AUTH"] = Constants.BitcoindRPCAuth;
            startInfo.EnvironmentVariables["BITCOIND_RPC_USER"] = Constants.BitcoindRPCUser;
            startInfo.EnvironmentVariables["BITCOIND_RPC_PASS"] = Constants.BitcoindRPCPass;

            _process = Process.Start(startInfo);
            if (_process is null) throw new InvalidOperationException("Failed to start server");

            _process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is null)
                    _outputComplete.Set();
                else
                    WriteOutput(e.Data);
            };
            
            _process.BeginOutputReadLine();

            _process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is null)
                    _errorComplete.Set();
                else
                    WriteOutput(e.Data);
            };

        }

        public string Output
        {
            get
            {
                lock (_sync)
                {
                    return _output.ToString();
                }
            }
        }

        public bool HasExited => _process.HasExited;

        public void Dispose()
        {
            try
            {
                _process.Kill();
                WaitForExit();
            }
            catch
            {
                // ignored
            }
        }

        public void WriteOutput(string line)
        {
            lock (_sync)
            {
                _output.WriteLine(line);
            }
        }

        public int WaitForExit(TimeSpan? timeout = null)
        {
            _process.WaitForExit((int) (timeout ?? Timeout.InfiniteTimeSpan).TotalMilliseconds);
            if (!_outputComplete.WaitOne(TimeSpan.FromSeconds(30)))
                throw new IOException("STDOUT did not complete in time");
            if (!_errorComplete.WaitOne(TimeSpan.FromSeconds(30)))
                throw new IOException("STDERR did not complete in time");
            return _process.ExitCode;
        }
    }
}