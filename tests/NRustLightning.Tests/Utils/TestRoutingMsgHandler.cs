using System;
using NRustLightning.Adaptors;
using NRustLightning.Interfaces;

namespace NRustLightning.Tests.Utils
{
    public class TestRoutingMsgHandler : IRoutingMsgHandler
    {
        private HandleNodeAnnouncement handleNodeAnnouncement;
        private HandleChannelAnnouncement handleChannelAnnouncement;
        private HandleChannelUpdate handleChannelUpdate;
        private HandleHTLCFailChannelUpdate handleHtlcFailChannelUpdate;
        private GetNextChannelAnnouncements getNextChannelAnnouncements;
        private GetNextNodeAnnouncements getNextNodeAnnouncements;
        private ShouldRequestFullSync shouldRequestFullSync;

        public TestRoutingMsgHandler()
        {
            handleNodeAnnouncement =
                (ref FFINodeAnnoucement msgPtr, ref FFILightningError error) =>
                {
                    var msg = msgPtr.ParseArray();
                    Console.WriteLine($"Received NodeAnnouncement {msg}");
                    return 1;
                };
        }

        public ref HandleNodeAnnouncement HandleNodeAnnouncement =>
        
            ref handleNodeAnnouncement;

        public ref HandleChannelAnnouncement HandleChannelAnnouncement =>
            ref handleChannelAnnouncement;

        public ref HandleChannelUpdate HandleChannelUpdate =>
            ref handleChannelUpdate;

        public ref HandleHTLCFailChannelUpdate HandleHtlcFailChannelUpdate =>
            ref handleHtlcFailChannelUpdate;

        public ref GetNextChannelAnnouncements GetNextChannelAnnouncements =>
            ref getNextChannelAnnouncements;

        public ref GetNextNodeAnnouncements GetNextNodeAnnouncements =>
            ref getNextNodeAnnouncements;

        public ref ShouldRequestFullSync ShouldRequestFullSync =>
            ref shouldRequestFullSync;
    }
}