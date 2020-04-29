using System;
using System.Buffers;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Server
{
    public class DuplexPipeSocketDescriptor : ISocketDescriptor
    {
        public UIntPtr Index { get; }

        SendData sendData;
        private DisconnectSocket disconnectSocket;

        private PipeWriter Output;
        /// <summary>
        /// If this is true, that means rust-lightning has told us
        /// not to call anymore WriteBufferSpaceAvail, ReadEvent, and SocketDisconnected.
        /// </summary>
        public bool Disconnected { get; set; }
        
        public DuplexPipeSocketDescriptor(UIntPtr index,  PipeWriter output, ILogger<DuplexPipeSocketDescriptor> logger)
        {
            Index = index;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            sendData = (ref FFIBytes data, byte resumeRead) =>
            {
                logger.LogDebug($"Writing: {Hex.Encode(data.AsSpan())}");
                Output.Write(data.AsSpan());
                return Disconnected ? (UIntPtr)0 : data.len;
            };
            disconnectSocket = () =>
            {
                if (!Disconnected) throw new Exception("rust-lightning has called disconnect_socket more than once.");
                Disconnected = true;
            };
        }

        public ref SendData SendData => ref sendData;

        public ref DisconnectSocket DisconnectSocket => ref disconnectSocket;
    }
}