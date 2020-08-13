using System.IO.Pipelines;
using System.Threading.Channels;
using NRustLightning.Interfaces;
using NRustLightning.Net;

namespace NRustLightning.Server.Interfaces
{
    public interface ISocketDescriptorFactory
    {
        (SocketDescriptor, ChannelReader<byte>) GetNewSocket(PipeWriter writer);
    }
}