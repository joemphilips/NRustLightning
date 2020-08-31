using NBitcoin;

namespace NRustLightning.Server.Events
{
    public class TxBroadcastEvent
    {
        public Transaction Tx { get; set; }
    }
}