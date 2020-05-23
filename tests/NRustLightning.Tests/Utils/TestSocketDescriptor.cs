using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Utils
{
    public class TestSocketDescriptor : ISocketDescriptor
    {
        public UIntPtr Index { get; }
        private SendData _sendData;
        private DisconnectSocket _disconnectSocket;
        private PipeWriter Output;
        
        /// <summary>
        /// If this is true, that means rust-lightning has told us
        /// not to call anymore WriteBufferSpaceAvail, ReadEvent, and SocketDisconnected.
        /// </summary>
        public bool Disconnected { get; set; }

        public TestSocketDescriptor(UIntPtr index, PipeWriter output)
        {
            Index = index;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            _sendData = (data, resumeRead) =>
            {
                Debug.Assert(data.AsSpan().SequenceEqual(data.AsArray()), "Span and memory must be same inside a delegate");
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