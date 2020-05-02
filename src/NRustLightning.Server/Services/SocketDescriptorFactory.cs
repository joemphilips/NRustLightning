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
        private UIntPtr CurrentNum = (UIntPtr)0;
        private Dictionary<int, ISocketDescriptor> Sockets = new Dictionary<int, ISocketDescriptor>();
        public SocketDescriptorFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public ISocketDescriptor GetNewSocket(PipeWriter writer)
        {
            CurrentNum = CurrentNum + 1;
            Sockets[(int)CurrentNum] = new DuplexPipeSocketDescriptor(CurrentNum, writer, loggerFactory.CreateLogger<DuplexPipeSocketDescriptor>());
            return Sockets[(int)CurrentNum];
        }
    }
}