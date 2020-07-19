using System;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace NRustLightning.Net.Sockets
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
            _eventArgs.UserToken = _awaitable;
            _eventArgs.Completed += (_, e) => { ((SocketAwaitable)e.UserToken).Complete(e.BytesTransferred, e.SocketError);};
        }

        public SocketAwaitable ReceiveAsync(Memory<byte> buffer)
        {
            _eventArgs.SetBuffer(buffer);
            
            if (!_socket.ReceiveAsync(_eventArgs))
            {
                _awaitable.Complete(_eventArgs.BytesTransferred, _eventArgs.SocketError);
            }

            return _awaitable;
        }
    }
}
