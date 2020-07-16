using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using NRustLightning.Interfaces;

namespace NRustLightning.Adaptors
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FFIResult
    {
        private enum Kind : uint
        {
            Ok,
            EmptyPointerProvided,
            InvalidDataLength,
            DeserializationFailure,
            BufferTooSmall,
            InternalError,
            PaymentSendFailure
        }

        private readonly Kind _kind;
        private readonly uint _id;
        public static (FFIResult, string) GetLastResult()
        {
            return LastResult.GetLastResult();
        }

        public bool IsSuccess => _kind == Kind.Ok;
        public bool IsBufferTooSmall => _kind == Kind.BufferTooSmall;
        public bool IsPaymentSendFailure => _kind == Kind.PaymentSendFailure;

        internal FFIResult Check()
        {
            if (IsSuccess) return this;
            
            var (lastResult, msg) = GetLastResult();

            // Check whether the last result kind and id are the same
            // We need to use both because successful results won't
            // bother setting the id (it avoids some synchronization)
            if (lastResult._kind == _kind && lastResult._id == _id)
                throw new FFIException($"FFI against rust-lightning failed ({_kind}), {msg?.TrimEnd()}", lastResult);

            throw new FFIException($"FFI against rust-lightning failed with {_kind}", lastResult);
        }

        private (string, PaymentSendFailureType) TryGetSendPaymentFailureMsg()
        {
            var (lastResult, msg) = GetLastResult();
            if (!(lastResult._kind == _kind && lastResult._id == _id))
            {
                return ("Failed to send payment for unknown reason", PaymentSendFailureType.Unknown);
            }
            if (msg.Contains("ParameterError"))
            {
                return (msg, PaymentSendFailureType.ParameterError);
            }

            if (msg.Contains("PathParameterError"))
            {
                return (msg, PaymentSendFailureType.PathParameterError);
            }

            if (msg.Contains("AllFailedRetrySafe"))
            {
                return (msg, PaymentSendFailureType.AllFailedRetrySafe);
            }
            if (msg.Contains("PartialFailure"))
            {
                return (msg, PaymentSendFailureType.PartialFailure);
            }

            this.Check();
            throw new Exception("Unreachable");
        }

        internal FFIResult CheckPaymentSendFailure()
        {
            var (msg, kind) = TryGetSendPaymentFailureMsg();
            throw new PaymentSendException(this, kind, msg);
        }
    }

    public class PaymentSendException : FFIException
    {
        public PaymentSendFailureType Kind;

        public PaymentSendException(FFIResult result, PaymentSendFailureType kind, string msg) : base(msg, result)
        {
            Kind = kind;
        }
    }

    public enum PaymentSendFailureType : uint
    {
        Unknown,
        ParameterError,
        PathParameterError,
        AllFailedRetrySafe,
        PartialFailure
    }

    public class FFIException : Exception
    {
        public FFIResult Result { get; }

        public FFIException(string msg, FFIResult result) : base(msg)
        {
            Result = result;
        }
    }
}