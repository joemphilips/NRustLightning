using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Common.Utils
{
    public static class TestUserConfig
    {
        public static UserConfig Default = UserConfig.GetDefault();
    }

    public class TestUserConfigProvider : IUserConfigProvider
    {
        public UserConfig GetUserConfig()
        {
            return TestUserConfig.Default;
        }
    }
}