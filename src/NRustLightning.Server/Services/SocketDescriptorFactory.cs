using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using NRustLightning.Interfaces;
using NRustLightning.Net;
using NRustLightning.Net.Sockets;
using NRustLightning.Server.Interfaces;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NRustLightning.Server.Services
{
    public class SocketDescriptorFactory : ISocketDescriptorFactory
    {
        private readonly ILoggerFactory loggerFactory;
        private int CurrentNum = 0;
        private Dictionary<int, SocketDescriptor> Sockets = new Dictionary<int, SocketDescriptor>();
        public SocketDescriptorFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public (SocketDescriptor, ChannelReader<byte>) GetNewSocket(PipeWriter writer)
        {
            Interlocked.Add(ref CurrentNum, 1);
            
            // We only ever need a channel of depth 1 here: if we returned a non-full write to the PeerManager,
            // we will eventually get notified that there is room in the socket to write new bytes, which will generate
            // an event. That event will be popped off the queue before we call WriteBufferSpaceAvail, ensuring that we
            // have room to push a new () if, during the WriteBufferSpaceAvail call, send_data() returns a non-full write.
            var writeAvail = Channel.CreateBounded<byte>(1);
            Sockets[CurrentNum] = new SocketDescriptor((UIntPtr)CurrentNum, writer, loggerFactory.CreateLogger<SocketDescriptor>(), writeAvail.Writer);
            return (Sockets[(int)CurrentNum], writeAvail.Reader);
        }
    }
}