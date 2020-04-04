using DotNetLightning.LDK.Adaptors;

namespace DotNetLightning.LDK.Interfaces
{
    public interface ILogger
    {
        ref Log Log { get; }
    }
    
}