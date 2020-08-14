using System.IO.Pipelines;
using System.Threading.Channels;

namespace NRustLightning.Net
{
    public interface ISocketDescriptorFactory
    {
        (SocketDescriptor, ChannelReader<byte>) GetNewSocket(PipeWriter writer);
    }
}