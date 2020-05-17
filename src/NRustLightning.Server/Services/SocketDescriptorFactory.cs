using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using Microsoft.Extensions.Logging;
using NRustLightning.Interfaces;
using NRustLightning.Server.Interfaces;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NRustLightning.Server.Services
{
    public class SocketDescriptorFactory : ISocketDescriptorFactory
    {
        private readonly ILoggerFactory loggerFactory;
        private int CurrentNum = 0;
        private Dictionary<int, ISocketDescriptor> Sockets = new Dictionary<int, ISocketDescriptor>();
        public SocketDescriptorFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public ISocketDescriptor GetNewSocket(PipeWriter writer)
        {
            Interlocked.Add(ref CurrentNum, 1);
            Sockets[CurrentNum] = new SocketDescriptor((UIntPtr)CurrentNum, writer, loggerFactory.CreateLogger<SocketDescriptor>());
            return Sockets[(int)CurrentNum];
        }
    }
}