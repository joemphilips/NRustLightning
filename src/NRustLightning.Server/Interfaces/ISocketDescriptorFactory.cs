using System.IO.Pipelines;
using NRustLightning.Interfaces;

namespace NRustLightning.Server.Interfaces
{
    public interface ISocketDescriptorFactory
    {
        ISocketDescriptor GetNewSocket(PipeWriter writer);
    }
}