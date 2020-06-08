namespace NRustLightning.Server.Authentication
{
    public static class LSATDefaults
    {
        /// <summary>
        /// Authorization scheme name.
        /// </summary>
        public const string Scheme = "LSAT";
        /// <summary>
        /// See https://github.com/lightninglabs/LSAT/blob/master/macaroons.md#service-capabilities
        /// </summary>
        public const string CapabilitiesConditionPrefix = "_capabilities";
    }
}