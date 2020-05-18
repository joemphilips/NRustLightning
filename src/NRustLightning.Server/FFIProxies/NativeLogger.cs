using System;
using Microsoft.Extensions.Logging;
using NBitcoin.Logging;
using NRustLightning.Adaptors;
using NRustLightning.Server.Extensions;
using INativeLogger = NRustLightning.Interfaces.ILogger;

namespace NRustLightning.Server.FFIProxies
{
    public class NativeLogger : INativeLogger
    {
        private readonly ILogger<NativeLogger> logInternal;
        private Log log;

        public NativeLogger(ILogger<NativeLogger> logInternal)
        {
            this.logInternal = logInternal;
            log = (ref FFILogRecord record) =>
            {
                if (record.level == FFILogLevel.Off) return;
                var msg =
                    $"log in rust-lightning: {record.Args}. module path: {record.ModulePath}. file: {record.File}. line: {record.line} level: {record.level}";
                
                logInternal.Log(record.level.AsLogLevel(), msg);
            };
        }

        public ref Log Log => ref log;
    }
    
}