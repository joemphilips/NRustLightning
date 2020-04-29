using System;
using System.Collections.Generic;
using System.IO.Pipelines;
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
            CurrentNum++;
            Sockets[CurrentNum] = new DuplexPipeSocketDescriptor((UIntPtr)CurrentNum, writer, loggerFactory.CreateLogger<DuplexPipeSocketDescriptor>());
            return Sockets[CurrentNum];
        }
    }
}