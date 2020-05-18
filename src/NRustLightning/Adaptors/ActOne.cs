using System;
using System.Runtime.CompilerServices;

namespace NRustLightning.Adaptors
{
    public unsafe struct ActOne
    {
        public fixed byte Data[50];

        public byte[] AsArray()
        {
            var result = new byte[50];
            for (int i = 0; i < 50; i++)
            {
                result[i] = Data[i];
            }
            return result;
        }
    }
}