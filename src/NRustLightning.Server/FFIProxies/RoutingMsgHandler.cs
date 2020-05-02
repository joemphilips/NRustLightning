using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Server.FFIProxies
{
    public class RoutingMsgHandler : IRoutingMsgHandler
    {
        private HandleNodeAnnouncement _handleNodeAnnouncement;
        private HandleChannelAnnouncement _handleChannelAnnouncement;
        private HandleChannelUpdate _handleChannelUpdate;
        private HandleHTLCFailChannelUpdate _handleHtlcFailChannelUpdate;
        private GetNextChannelAnnouncements _getNextChannelAnnouncements;
        private GetNextNodeAnnouncements _getNextNodeAnnouncements;
        private ShouldRequestFullSync _shouldRequestFullSync;
        public RoutingMsgHandler() {}
        public ref HandleNodeAnnouncement HandleNodeAnnouncement => ref _handleNodeAnnouncement;

        public ref HandleChannelAnnouncement HandleChannelAnnouncement => ref _handleChannelAnnouncement;

        public ref HandleChannelUpdate HandleChannelUpdate => ref _handleChannelUpdate;

        public ref HandleHTLCFailChannelUpdate HandleHtlcFailChannelUpdate => ref _handleHtlcFailChannelUpdate;

        public ref GetNextChannelAnnouncements GetNextChannelAnnouncements => ref _getNextChannelAnnouncements;

        public ref GetNextNodeAnnouncements GetNextNodeAnnouncements => ref _getNextNodeAnnouncements;

        public ref ShouldRequestFullSync ShouldRequestFullSync => ref _shouldRequestFullSync;
    }
}