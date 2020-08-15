using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Channels;

namespace NRustLightning.Net
{
    public class SocketDescriptorFactory : ISocketDescriptorFactory
    {
        private int CurrentNum = 0;
        private Dictionary<int, SocketDescriptor> Sockets = new Dictionary<int, SocketDescriptor>();
        public SocketDescriptorFactory()
        {
        }
        public (SocketDescriptor, ChannelReader<byte>) GetNewSocket(PipeWriter writer)
        {
            Interlocked.Increment(ref CurrentNum);
            
            // We only ever need a channel of depth 1 here: if we returned a non-full write to the PeerManager,
            // we will eventually get notified that there is room in the socket to write new bytes, which will generate
            // an event. That event will be popped off the queue before we call WriteBufferSpaceAvail, ensuring that we
            // have room to push a new `1` if, during the WriteBufferSpaceAvail call, send_data() returns a non-full write.
            var writeAvail = Channel.CreateBounded<byte>(1);
            Sockets[CurrentNum] = new SocketDescriptor((UIntPtr)CurrentNum, writer, writeAvail.Writer);
            return (Sockets[(int)CurrentNum], writeAvail.Reader);
        }
    }
}