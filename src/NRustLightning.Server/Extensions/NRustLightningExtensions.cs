using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using NBitcoin;
using NRustLightning.Adaptors;
using Network = NBitcoin.Network;

namespace NRustLightning.Server.Extensions
{
    public static class NRustLightningExtensions
    {
        public static LogLevel AsLogLevel(this FFILogLevel l) => l switch
        {
            FFILogLevel.Debug => LogLevel.Debug,
            FFILogLevel.Error => LogLevel.Error,
            FFILogLevel.Info => LogLevel.Information,
            FFILogLevel.Off => LogLevel.None,
            FFILogLevel.Trace => LogLevel.Trace,
            FFILogLevel.Warn => LogLevel.Warning,
            _ => throw new System.Exception("unreachable")
        };

        public static FSharpOption<T> ToFSharpOption<T>(this Nullable<T> val) where T : struct
        {
            return val is null ? FSharpOption<T>.None : FSharpOption<T>.Some(val.Value);
        }

        public static FSharpList<T> ToFSharpList<T>(this IEnumerable<T> val)
        {
            if (val == null) throw new ArgumentNullException(nameof(val));
            var r = FSharpList<T>.Empty;
            foreach (var v in val)
            {
                r = FSharpList<T>.Cons(v, r);
            }
            return r;
        }

        public static T? ToNullable<T>(this FSharpOption<T> val) where T : struct
        {
            return (val is null) || FSharpOption<T>.None.Equals(val) ? (T?)null : val.Value;
        }
        public static T GetOrDefault<T>(this FSharpOption<T> val)
        {
            return (val is null) || FSharpOption<T>.None.Equals(val) ? default : val.Value;
        }
    }
}