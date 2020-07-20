using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NRustLightning.Net.Sockets
{
    public class SocketSender
    {
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _eventArgs = new SocketAsyncEventArgs();
        private readonly SocketAwaitable _awaitable;

        private List<ArraySegment<byte>>? _bufferList;

        public SocketSender(Socket socket, PipeScheduler scheduler)
        {
            _socket = socket;
            _awaitable = new SocketAwaitable(scheduler);
            _eventArgs.UserToken = _awaitable;
            _eventArgs.Completed += (_, e) =>
                ((SocketAwaitable) e.UserToken).Complete(e.BytesTransferred, e.SocketError);
        }

        public SocketAwaitable SendAsync(in ReadOnlySequence<byte> buffers)
        {
            if (buffers.IsSingleSegment)
            {
                return SendAsync(buffers.First);
            }

            if (!_eventArgs.MemoryBuffer.Equals(Memory<byte>.Empty))
            {
                _eventArgs.SetBuffer(null, 0, 0);
            }

            _eventArgs.BufferList = GetBufferList(buffers);
            if (!_socket.SendAsync(_eventArgs))
            {
                _awaitable.Complete(_eventArgs.BytesTransferred, _eventArgs.SocketError);
            }

            return _awaitable;
        }

        private SocketAwaitable SendAsync(ReadOnlyMemory<byte> memory)
        {
            if (_eventArgs.BufferList != null)
            {
                _eventArgs.BufferList = null;
            }
            _eventArgs.SetBuffer(MemoryMarshal.AsMemory(memory));
            if (!_socket.SendAsync(_eventArgs))
            {
                _awaitable.Complete(_eventArgs.BytesTransferred, _eventArgs.SocketError);
            }

            return _awaitable;
        }

        private List<ArraySegment<byte>> GetBufferList(in ReadOnlySequence<byte> buffer)
        {
            Debug.Assert(!buffer.IsEmpty);
            Debug.Assert(!buffer.IsSingleSegment);
            if (_bufferList == null)
            {
                _bufferList = new List<ArraySegment<byte>>();
            }
            else
            {
                _bufferList.Clear();
            }

            foreach (var b in buffer)
            {
                _bufferList.Add(b.GetArray());
            }

            return _bufferList;
        }
    }
}