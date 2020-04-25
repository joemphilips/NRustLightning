namespace NRustLightning.Server.Interfaces
{
    public interface IKeysRepository
    {
        byte[] GetNodeSecret();
    }
}