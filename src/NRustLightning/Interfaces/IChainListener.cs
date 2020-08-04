using DotNetLightning.Utils;
using NBitcoin;

namespace NRustLightning.Interfaces
{
    public interface IChainListener
    {
        /// <summary>
        /// Notifies a listener that a block was connected.
        /// This interface will make implementation simpler by
        ///
        /// The last argument is used only in ManyChannelMonitor.
        /// </summary>
        public void BlockConnected(Block block, uint height, Primitives.LNOutPoint key = null);
        
        public void BlockDisconnected(BlockHeader header, uint height, Primitives.LNOutPoint key = null);
    }
}