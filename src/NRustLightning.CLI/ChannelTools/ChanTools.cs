using System.CommandLine;

namespace NRustLightning.CLI.ChannelTools
{
    public class ChanTools
    {
        /// <summary>
        /// Get common options for `chantools` subcommand.
        /// </summary>
        /// <returns></returns>
        public Option[] GetOptions() =>
        new Option[]
        {
            new Option(""), 
        };

        public Command ChanBackup()
        {
            var opts = 
        }
    }
}