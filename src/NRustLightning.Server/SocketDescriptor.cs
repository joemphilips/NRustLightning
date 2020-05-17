using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;
using NRustLightning.Utils;

namespace NRustLightning.Server
{
    public class DuplexPipeSocketDescriptor : ISocketDescriptor
    {
        public UIntPtr Index { get; }

        /// <summary>
        /// This will be called from rust-lighting, and it writes a data to an output buffer.
        /// But it does not flush it since flushing requires asynchronous operation.
        /// You must call `FlushAsync` on caller side.
        /// </summary>
        SendData sendData;
        private DisconnectSocket disconnectSocket;

        public PipeWriter Output;

        /// <summary>
        /// If this is true, that means rust-lightning has told us
        /// not to call anymore WriteBufferSpaceAvail, ReadEvent, and SocketDisconnected.
        /// </summary>
        public bool Disconnected { get; set; } = false;
        
        public DuplexPipeSocketDescriptor(UIntPtr index,  PipeWriter output, ILogger<DuplexPipeSocketDescriptor> logger)
        {
            Index = index;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            sendData = (data, resumeRead) =>
            {
                Debug.Assert(data.AsSpan().SequenceEqual(data.AsArray()), "Span and memory must be same inside a delegate", $"span: {Hex.Encode(data.AsSpan())}, array: {Hex.Encode(data.AsArray())}");
                Output.Write(data.AsSpan());
                var r = Output.FlushAsync().GetAwaiter().GetResult();
                return Disconnected ? (UIntPtr)0 : data.len;
            };
            disconnectSocket = () =>
            {
                logger.LogDebug($"Disconnecting socket {Index}");
                if (Disconnected) throw new Exception("rust-lightning has called disconnect_socket more than once.");
                Disconnected = true;
            };
        }

        public ref SendData SendData => ref sendData;

        public ref DisconnectSocket DisconnectSocket => ref disconnectSocket;
    }
}