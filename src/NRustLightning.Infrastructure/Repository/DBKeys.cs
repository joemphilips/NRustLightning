namespace NRustLightning.Infrastructure.Repository
{
    public static class DBKeys
    {
        # region keys for the table
        public const string HashToPreimage = "hp";
        public const string HashToInvoice = "hi";
        public const string OutpointsToStaticOutputDescriptor = "os";
        
        // ---- items which does not have value (only keys)
        public const string RemoteEndPoints = "re";
        // ----
        
        public const string ChannelManager = "cm";
        public const string ManyChannelMonitor = "mc";
        public const string NetworkGraph = "n";
        # endregion

        # region keys for singleton value in the table
        public const string ChannelManagerVersion = "v1";
        public const string ManyChannelMonitorVersion = "v1";
        public const string NetworkGraphVersion = "v1";
        # endregion
    }
}