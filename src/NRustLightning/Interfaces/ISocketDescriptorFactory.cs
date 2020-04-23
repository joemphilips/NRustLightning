namespace NRustLightning.Interfaces
{
    public interface ISocketDescriptorFactory
    {
        ISocketDescriptor GetNewSocket();
        ISocketDescriptor GetSocket(int index);
    }
}