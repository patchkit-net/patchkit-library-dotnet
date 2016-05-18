using System;
using System.Threading;

namespace PatchKit.API
{
    /// <summary>
    /// Asynchronus API result.
    /// </summary>
    /// <typeparam name="T">The type of API data.</typeparam>
    public class PatchKitAPIAsyncResult<T> : IAsyncResult
    {
        private readonly object _locker = new object();

        private readonly object _state;

        private readonly AsyncCallback _callback;

        private readonly PatchKitAPICancellationTokenSource _cancellationTokenSource = new PatchKitAPICancellationTokenSource();

        private ManualResetEvent _manualResetEvent;

        internal T Result { get; private set; }

        internal Exception Exception { get; private set; }

        public bool IsCompleted { get; private set; }

        public bool HasBeenCancelled { get; private set; }

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

		internal PatchKitAPIAsyncResult(Func<PatchKitAPICancellationToken, T> workToBeDone, AsyncCallback callback, object state)
		{
            _callback = callback;
            _state = state;
            IsCompleted = false;

		    QueueWorkOnThreadPool(workToBeDone);
		}

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        private void QueueWorkOnThreadPool(Func<PatchKitAPICancellationToken, T> workToBeDone)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    Result = workToBeDone(_cancellationTokenSource);
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
}
