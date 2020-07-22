using System.Collections.Concurrent;

namespace NRustLightning.Infrastructure.Utils
{
    public class FixedSizeQueue<T> : ConcurrentQueue<T>
    {
        private readonly object syncObject = new object();

        public int Size { get; private set; }

        public FixedSizeQueue(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    base.TryDequeue(out _);
                }
            }
        }
    }
}