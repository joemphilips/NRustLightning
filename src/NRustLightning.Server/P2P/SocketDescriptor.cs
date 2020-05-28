using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Server.P2P
{
    public class SocketDescriptor : ISocketDescriptor
    {
        public UIntPtr Index { get; }
        public ChannelWriter<byte> WriteAvail { get; }

        /// <summary>
        /// This will be called from rust-lighting, and it writes a data to an output buffer.
        /// But it does not flush it since flushing requires asynchronous operation.
        /// You must call `FlushAsync` on caller side.
        /// </summary>
        SendData sendData;
        private DisconnectSocket disconnectSocket;

        public PipeWriter Output;
        private readonly ILogger<SocketDescriptor> _logger;

        private int _blockDisconnectSocket = 0;
        /// <summary>
        ///  When we are told by rust-lightning to disconnect, we can't return to rust-lightning until we
        /// are sure we won't call any more read/write PeerManager functions with the same connection.
        /// This is set to true if we're in such a condition (with disconnect checked before with the
        /// top-level mutex held) and false when we can return.
        /// </summary>
        public bool BlockDisconnectSocket
            {
                get => _blockDisconnectSocket == 1;
                set => Interlocked.Exchange(ref _blockDisconnectSocket, (value ? 1 : 0));
            }

        private int _readPaused = 0;
        public bool ReadPaused 
            {
                get => _readPaused == 1;
                set => Interlocked.Exchange(ref _readPaused, (value ? 1 : 0));
            }
        
        private int _rlRequestedDisconnect = 0;
        /// <summary>
        /// If this is true, that means rust-lightning has told us
        /// not to call anymore WriteBufferSpaceAvail, ReadEvent, and SocketDisconnected.
        /// </summary>
        public bool Disconnected
            {
                get => _rlRequestedDisconnect == 1;
                set => Interlocked.Exchange(ref _rlRequestedDisconnect, (value ? 1 : 0));
            }
        public SocketDescriptor(UIntPtr index,  PipeWriter output, ILogger<SocketDescriptor> logger, ChannelWriter<byte> writeAvail)
        {
            Index = index;
            WriteAvail = writeAvail;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            _logger = logger;
            sendData = (data, resumeRead) =>
            {
                if (resumeRead == 1 && ReadPaused)
                {
                    ReadPaused = false;
                }

                if (data.len == UIntPtr.Zero) return UIntPtr.Zero;
                Output.Write(data.AsSpan());
                var flushTask = Output.FlushAsync();
                if (!flushTask.IsCompleted)
                {
                    flushTask.GetAwaiter().GetResult();
                }

                WriteAvail.TryWrite(1);
                return (Disconnected) ? (UIntPtr)0 : data.len;
            };
            disconnectSocket = () =>
            {
                logger.LogDebug($"Disconnecting socket {Index}");
                if (Disconnected) throw new Exception("rust-lightning has called disconnect_socket more than once.");
                Disconnected = true;
                ReadPaused = true;

                WriteAvail.TryWrite(1);
                // happy-path return:
                if (!BlockDisconnectSocket)
                {
                    return; 
                }
                while (BlockDisconnectSocket)
                {
                    Task.Delay(10).GetAwaiter().GetResult();
                }
            };
        }

        public ref SendData SendData => ref sendData;

        public ref DisconnectSocket DisconnectSocket => ref disconnectSocket;
    }
}