using NRustLightning.Adaptors;

namespace NRustLightning.Interfaces
{
    public interface IRoutingMsgHandler
    {
        ref HandleNodeAnnouncement HandleNodeAnnouncement { get; }
        ref HandleChannelAnnouncement HandleChannelAnnouncement { get; }
        ref HandleChannelUpdate HandleChannelUpdate { get; }
        ref HandleHTLCFailChannelUpdate HandleHtlcFailChannelUpdate { get; }
        ref GetNextChannelAnnouncements GetNextChannelAnnouncements { get; }
        ref GetNextNodeAnnouncements GetNextNodeAnnouncements { get; }
        ref ShouldRequestFullSync ShouldRequestFullSync { get; }
    }
}