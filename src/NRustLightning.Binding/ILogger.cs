using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NRustLightning.Binding
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log(IntPtr this_arg, LDKLogger ldkLgger);
    public class LoggerHandle : SafeHandle
    {
        private LoggerHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            if (handle == IntPtr.Zero) return true;

            var h = handle;
            handle = IntPtr.Zero;

            return true;
        }
    }
    public enum FFILogLevel : int
    {
        Off,
        Error,
        Warn,
        Info,
        Debug,
        Trace
    }
    public interface ILogger
    {
        void Log(string msg);
    }
    public class LoggerDelegateHolder : IDisposable
    {
        private readonly ILogger _logger;

        private LDKLogger _ldklogger;
        private GCHandle _handle;
        private bool _disposed;

        public unsafe LoggerDelegateHolder(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Action<IntPtr, IntPtr> log = (thisArg, record) =>
            {
                var str = Marshal.PtrToStringUTF8(record);
                if (str != null)
                    logger.Log(str);
            };
            _ldklogger.log = (IntPtr)Unsafe.AsPointer(ref log);
            _handle = GCHandle.Alloc(_ldklogger);
            _disposed = false;
        }

        public void Dispose()
        {
        }

    }
}