using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NRustLightning.Utils
{
  public static class StreamExtensions
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this Stream stream, ref T value)
    where T: unmanaged
    {
      var tSpan = MemoryMarshal.CreateSpan(ref value, 1);
      var span = MemoryMarshal.AsBytes(tSpan);
      stream.Write(span);
    }
  }
}