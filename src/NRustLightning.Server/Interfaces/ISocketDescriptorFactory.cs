using System.IO.Pipelines;
using System.Threading.Channels;
using NRustLightning.Interfaces;
using NRustLightning.Server.P2P;

namespace NRustLightning.Server.Interfaces
{
    public interface ISocketDescriptorFactory
    {
        (SocketDescriptor, ChannelReader<byte>) GetNewSocket(PipeWriter writer);
    }
}