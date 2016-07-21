using System;
using System.Threading;
using JetBrains.Annotations;

namespace PatchKit.Async
{
    /// <summary>
    /// Asynchronus operation result.
    /// </summary>
    /// <typeparam name="T">Type of data returned by operation.</typeparam>
    public class AsyncResult<T> : ICancellableAsyncResult
    {
        [CanBeNull]
        private readonly AsyncCancellationTokenSource _cancellationTokenSource;

        private readonly object _statusLocker = new object();

        private readonly object _asyncWaitHandleLocker = new object();

        private ManualResetEvent _asyncWaitHandle;

        private readonly object _asyncState;

        private readonly CancellableAsyncCallback _asyncCallback;

        private T _result;

        private Exception _exception;

        public bool IsCompleted { get; private set; }

        public bool IsCancelled { get; private set; }

        public object AsyncState
        {
            get { return _asyncState; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        /// <summary>
        /// Result of async opperation.
        /// </summary>
        public T Result
        {
            get
            {
                lock (_statusLocker)
                {
                    if (!IsCompleted)
                    {
                        throw new InvalidOperationException(
                            "Cannot access the result when async operation isn't completed.");
                    }
                }

                return _result;
            }
        }

        /// <summary>
        /// Exception which was raised by async operation.
        /// </summary>
        public Exception Exception
        {
            get
            {
                lock (_statusLocker)
                {
                    if (!IsCompleted)
                    {
                        throw new InvalidOperationException(
                            "Cannot access the exception when async operation isn't completed.");
                    }
                }

                return _exception;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                lock (_asyncWaitHandleLocker)
                {
                    if (_asyncWaitHandle == null)
                    {
                        _asyncWaitHandle = new ManualResetEvent(false);
                    }
                    if (IsCompleted)
                    {
                        _asyncWaitHandle.Set();
                    }
                }
                return _asyncWaitHandle;
            }
        }

        public bool CanBeCancelled
        {
            get
            {
                if (_cancellationTokenSource == null)
                {
                    return false;
                }

                lock (_statusLocker)
                {
                    return !IsCompleted;
                }
            }
        }

        private AsyncResult(CancellableAsyncCallback asyncCallback, object asyncState)
        {
            _asyncCallback = asyncCallback;
            _asyncState = asyncState;
            IsCompleted = false;
        }

        public AsyncResult([NotNull] Func<T> work, AsyncCallback callback = null, object state = null) : this(ar =>
        {
            if (callback != null)
            {
                callback(ar);
            }
        }, state)
        {
            if (work == null)
            {
                throw new ArgumentNullException("work");
            }

            QueueWorkOnThreadPool(work);
        }

        public AsyncResult([NotNull] Func<AsyncCancellationToken, T> work, CancellableAsyncCallback callback = null, object state = null) : this(callback, state)
        {
            if (work == null)
            {
                throw new ArgumentNullException("work");
            }

            _cancellationTokenSource = new AsyncCancellationTokenSource();

            QueueWorkOnThreadPool(() => work(_cancellationTokenSource.Token));
        }

        public bool Cancel()
        {
            if (_cancellationTokenSource == null)
            {
                return false;
            }

            lock (_statusLocker)
            {
                if (IsCompleted)
                {
                    return false;
                }

                _cancellationTokenSource.Cancel();

                return true;
            }
        }

        private void QueueWorkOnThreadPool(Func<T> work)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    _result = work();
                }
                catch (OperationCanceledException)
                {
                    lock (_statusLocker)
                    {
                        IsCancelled = true;
                    }
                }
                catch (Exception exception)
                {
                    _exception = exception;
                }
                finally
                {
                    lock (_statusLocker)
                    {
                        IsCompleted = true;
                    }

                    lock (_asyncWaitHandleLocker)
                    {
                        if (_asyncWaitHandle != null)
                        {
                            _asyncWaitHandle.Set();
                        }
                    }

                    if (_asyncCallback != null)
                    {
                        _asyncCallback(this);
                    }
                }
            });
        }
    }

    public class AsyncResult : AsyncResult<object>
    {
        public AsyncResult([NotNull] Action work, AsyncCallback callback = null, object state = null) : base(() =>
        {
            work();
            return null;
        }, callback, state)
        {
        }

        public AsyncResult([NotNull] Action<AsyncCancellationToken> work, CancellableAsyncCallback callback = null, object state = null) : base(
            cancellationToken =>
            {
                work(cancellationToken);
                return null;
            }, callback, state)
        {
        }
    }
}
