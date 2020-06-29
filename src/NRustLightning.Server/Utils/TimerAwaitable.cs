using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NRustLightning.Server.Utils
{
    public class TimerAwaitable : IDisposable, ICriticalNotifyCompletion
    {
        private Timer _timer;
        private Action _callback;
        private static readonly Action _callbackCompleted = () => { };
        private readonly TimeSpan _dueTime;
        private readonly TimeSpan _period;

        private bool _disposed;
        private bool _running = true;
        private readonly object _lockObj = new object();

        public TimerAwaitable(TimeSpan dueTime, TimeSpan period)
        {
            _dueTime = dueTime;
            _period = period;
        }

        public void Start()
        {
            if (_timer == null)
            {
                lock (_lockObj)
                {
                    if (_disposed)
                        return;
                    if (_timer == null)
                    {
                        _timer = new Timer(state => ((TimerAwaitable)state).Tick(), this, _dueTime, _period);
                    }
                }
            }
        }

        public TimerAwaitable GetAwaiter() => this;
        public bool IsCompleted => ReferenceEquals(_callback, _callbackCompleted);

        public bool GetResult()
        {
            _callback = null;
            return _running;
        }

        private void Tick()
        {
            var cont = Interlocked.Exchange(ref _callback, _callbackCompleted);
            cont?.Invoke();
        }
        public void OnCompleted(Action continuation)
        {
            if (ReferenceEquals(_callback, _callbackCompleted) ||
                ReferenceEquals(Interlocked.CompareExchange(ref _callback, continuation, null), _callbackCompleted))
            {
                Task.Run(continuation);
            }
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }

        public void Stop()
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                _running = false;
            }
            Tick();
        }
        public void Dispose()
        {
            lock (_lockObj)
            {
                _disposed = true;
                _timer?.Dispose();
                _timer = null;
            }
        }
    }
}