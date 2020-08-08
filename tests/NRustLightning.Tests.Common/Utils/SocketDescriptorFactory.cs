using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Common.Utils
{
    public interface ISocketDescriptorFactory
    {
        ISocketDescriptor GetNewSocket();
        ISocketDescriptor GetSocket(int index);
    }
    public class SocketDescriptorFactory : ISocketDescriptorFactory
    {
        private int CurrentNum = 0;
        private Dictionary<int, ISocketDescriptor> Sockets = new Dictionary<int, ISocketDescriptor>();
        public ISocketDescriptor GetNewSocket()
        {
            CurrentNum++;
            Sockets[CurrentNum] = new TestSocketDescriptor((UIntPtr)CurrentNum, new Pipe().Writer);
            return Sockets[CurrentNum];
        }

        public ISocketDescriptor GetSocket(int index)
        {
            throw new NotImplementedException();
        }
    }
}