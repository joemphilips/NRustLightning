using System;
using System.IO;

namespace NRustLightning.Adaptors
{
    public unsafe struct Array32
    {
        public fixed byte Data[32];
        public byte[] AsArray()
        {
            var result = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                result[i] = Data[i];
            }
            return result;
        }

    }
}