namespace DotNetLightning.LDK.Primitives
{
    internal delegate void broadcast_transaction_ptr();
    internal ref struct FFIManyChannelMonitor
    {
        private broadcast_transaction_ptr ptr;
    }
}