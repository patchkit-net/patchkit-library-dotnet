using System;
using System.Threading;
using JetBrains.Annotations;

namespace PatchKit.API.Async
{
    /// <summary>
    /// Asynchronus operation result.
    /// </summary>
    /// <typeparam name="T">Type of data returned by operation.</typeparam>
    public class AsyncResult<T> : ICancellableAsyncResult
    {
        private readonly object _locker = new object();

        private readonly object _state;

        private readonly CancellableAsyncCallback _callback;

        private ManualResetEvent _manualResetEvent;

        public T Result { get; private set; }

        public Exception Exception { get; private set; }

        public bool IsCompleted { get; private set; }

        public bool HasBeenCancelled { get; private set; }

        public readonly AsyncCancellationTokenSource CancellationTokenSource = new AsyncCancellationTokenSource();

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                lock (_locker)
                {
                    if (_manualResetEvent == null)
                    {
                        _manualResetEvent = new ManualResetEvent(false);
                    }
                    if (IsCompleted)
                    {
                        _manualResetEvent.Set();
                    }
                }
                return _manualResetEvent;
            }
        }

        public object AsyncState
        {
            get { return _state; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        private AsyncResult(CancellableAsyncCallback callback, object state)
        {
            _callback = callback;
            _state = state;
            IsCompleted = false;
        }

        public AsyncResult([NotNull] Func<T> work, AsyncCallback callback, object state) : this(ar => callback(ar), state)
        {
            if (work == null)
            {
                throw new ArgumentNullException("work");
            }

            QueueWorkOnThreadPool(work);
        }

        public AsyncResult([NotNull] Func<AsyncCancellationToken, T> work, CancellableAsyncCallback callback, object state) : this(callback, state)
        {
            if (work == null)
            {
                throw new ArgumentNullException("work");
            }

            QueueWorkOnThreadPool(() => work(CancellationTokenSource.Token));
        }

        public void Cancel()
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("You cannot cancel already completed operation.");
            }
            
            CancellationTokenSource.Cancel();
        }

        private void QueueWorkOnThreadPool(Func<T> work)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    Result = work();
                }
                catch (OperationCanceledException)
                {
                    HasBeenCancelled = true;
                }
                catch (Exception exception)
                {
                    Exception = exception;
                }
                finally
                {
                    IsCompleted = true;

                    lock (_locker)
                    {
                        if (_manualResetEvent != null)
                        {
                            _manualResetEvent.Set();
                        }
                    }

                    if (_callback != null)
                    {
                        _callback(this);
                    }
                }
            });
        }
    }

    public class AsyncResult : AsyncResult<object>
    {
        public AsyncResult(Func<object> work, AsyncCallback callback, object state) : base(work, callback, state)
        {
        }

        public AsyncResult(Func<AsyncCancellationToken, object> work, CancellableAsyncCallback callback, object state) : base(work, callback, state)
        {
        }
    }
}
