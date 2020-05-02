using System;
using System.IO;

namespace NRustLightning.Server.Tests.Support
{
    public class DataPath : IDisposable
    {
        private readonly string _dir;

        public DataPath()
        {
            _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
            Directory.CreateDirectory(_dir);
        }
        public void Dispose()
        {
            if (Directory.Exists(_dir)) Directory.Delete(_dir, true);
        }

        public static implicit operator string(DataPath @this) => @this._dir;
        public override string ToString() => _dir;
    }
}