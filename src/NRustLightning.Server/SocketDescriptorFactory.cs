using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using NRustLightning.Interfaces;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server
{
    public class SocketDescriptorFactory : ISocketDescriptorFactory
    {
        private int CurrentNum = 0;
        private Dictionary<int, ISocketDescriptor> Sockets;
        public ISocketDescriptor GetNewSocket(PipeWriter writer)
        {
            CurrentNum++;
            Sockets[CurrentNum] = new DuplexPipeSocketDescriptor((UIntPtr)CurrentNum, writer);
            return Sockets[CurrentNum];
        }
    }
}