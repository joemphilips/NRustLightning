using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNetLightning.LDK
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ChaChaState
    {
        private uint a;
        private uint b;
        private uint c;
        private uint d;
    }
    [StructLayout((LayoutKind.Sequential))]
    internal struct ChaCha20
    {
        private ChaChaState state;
        private byte OutPut;
        private UIntPtr Offset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct Poly1305
    {
        fixed uint r[5];
        fixed uint h[5];
        fixed uint pad[4];
        private UIntPtr leftover;
        private fixed byte buffer[16];
        private byte finalized;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct ChaCha20Poly1305RFCState
    {
        private ChaCha20 cipher;
        private Poly1305 mac;
        private byte finished;
        private UIntPtr data_len;
        private ulong aad_len;
    }
    public sealed class ChaCha20Poly1305
    {
        private ChaCha20Poly1305RFCState _state;
        private int _tagSize = 16;
        public ChaCha20Poly1305()
        {
            _state = new ChaCha20Poly1305RFCState();
        }

        private protected unsafe void EncryptCore(
            ReadOnlySpan<byte> plainText,
            Span<byte> cipherText,
            Span<byte> tag
            )
        {
            Debug.Assert(cipherText.Length == plainText.Length);
            fixed (ChaCha20Poly1305RFCState* s = &_state)
            fixed (byte* inputPtr = plainText)
            fixed (byte* outputPtr = cipherText)
            fixed (byte* t = tag)
            {
                Interop.encrypt_ffi(s, inputPtr, (ulong)plainText.Length, outputPtr,(ulong)cipherText.Length, t, (ulong)tag.Length);
            }
        }

        public byte[] Encrypt(ReadOnlySpan<byte> plainText)
        {
            if (plainText.Length > int.MaxValue - _tagSize)
                throw new ArgumentException($"plainText too long {plainText.Length}");
            var cipherText = new byte[plainText.Length];
            var tag = new byte[_tagSize];
            EncryptCore(plainText, cipherText, tag);
            return cipherText;
        }
    }
}