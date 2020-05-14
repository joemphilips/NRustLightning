using System;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace NRustLightning.Server.P2P.Sockets
{
    public class SocketReceiver
    {
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        private readonly SocketAwaitable _awaitable;

        public SocketReceiver(Socket socket, PipeScheduler scheduler)
        {
            _socket = socket;
            _awaitable = new SocketAwaitable(scheduler);
        }

        public SocketAwaitable ReceiveAsync(Memory<byte> buffer)
        {
            _eventArgs.SetBuffer(buffer);
            var segment = buffer.GetArray();
            
            _eventArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
            if (!_socket.ReceiveAsync(_eventArgs))
            {
                _awaitable.Complete(_eventArgs.BytesTransferred, _eventArgs.SocketError);
            }

            return _awaitable;
        }
    }
}
