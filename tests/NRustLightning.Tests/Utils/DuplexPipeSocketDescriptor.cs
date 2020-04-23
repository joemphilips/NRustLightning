using System;
using System.Buffers;
using System.IO.Pipelines;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Utils
{
    public class DuplexPipeSocketDescriptor : ISocketDescriptor
    {
        public UIntPtr Index { get; }
        private SendData _sendData;
        private DisconnectSocket _disconnectSocket;
        private PipeWriter Output;
        
        /// <summary>
        /// If this is true, that means rust-lightning has told us
        /// not to call anymore WriteBufferSpaceAvail, ReadEvent, and SocketDisconnected.
        /// </summary>
        public bool Disconnected;

        public DuplexPipeSocketDescriptor(UIntPtr index, PipeWriter output)
        {
            Index = index;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            _sendData = (ref FFIBytes data, byte resumeRead) =>
            {
                Output.Write(data.AsSpan());
                return Disconnected ? (UIntPtr)0 : data.len;
            };
            _disconnectSocket = () =>
            {
                if (!Disconnected) throw new Exception("rust-lightning has called disconnect_socket more than once.");
                Disconnected = true;
            };
        }

        public ref SendData SendData => ref _sendData;

        public ref DisconnectSocket DisconnectSocket => ref _disconnectSocket;
    }
}