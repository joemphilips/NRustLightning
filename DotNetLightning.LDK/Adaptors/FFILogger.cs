using System;
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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public readonly ref struct FFILogRecord
    {
        /// The verbosity level of the message.
        public readonly FFILogLevel level;

        /// The message body.
        private readonly IntPtr args;

        /// The module path of the message.
        private readonly IntPtr module_path;

        /// The source file containing the message.
        private readonly IntPtr file;

        /// The line containing the message.
        public readonly uint line;

        public string Args => Marshal.PtrToStringUTF8(args);
        public string ModulePath => Marshal.PtrToStringUTF8(module_path);
        public string File => Marshal.PtrToStringUTF8(file);

    }
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log(ref FFILogRecord record);
    
}