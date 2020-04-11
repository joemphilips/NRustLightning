using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    public enum ChainError : uint
    {
        /// Client doesn't support UTXO lookup (but the chain hash matches our genesis block hash)
        NotSupported,
        /// Chain isn't the one watched
        NotWatched,
        /// Tx doesn't exist or is unconfirmed
        UnknownTx,
    }
}