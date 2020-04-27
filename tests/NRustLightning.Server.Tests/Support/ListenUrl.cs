using System.Threading;

namespace NRustLightning.Server.Tests.Support
{
    class ListenUrl
    {
        private static int _nextPort = 50000;
        private readonly string _value;

        public ListenUrl()
        {
            _value = $"http://localhost:{GetNextPort()}";
        }

        private static int GetNextPort() => Interlocked.Increment(ref _nextPort);
        public static implicit operator string(ListenUrl @this) => @this._value;
        public override string ToString()
        {
            return _value;
        }
    }
}