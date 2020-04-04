using System.Reflection;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK.Adaptors
{
    public enum FFILogLevel : int
    {
        Off,
        Error,
        Warn,
        Info,
        Debug,
        Trace
    }
    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFILogRecord
    {
        /// The verbosity level of the message.
        public FFILogLevel level;

        /// The message body.
        public string args;

        /// The module path of the message.
        public string module_path;

        /// The source file containing the message.
        public string file;

        /// The line containing the message.
        public uint line;
    }
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log(ref FFILogger self, ref FFILogRecord record);
    
    [StructLayout(LayoutKind.Sequential)]
    public ref struct FFILogger
    {
        internal Log log;
    }
}