using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Binding
{
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
    public interface ILogger : IDisposable
    {
        void Log(IntPtr record);
    }
    
    public class Logger : IDisposable, ILogger
    {
        public void Dispose()
        {
        }
    }
}