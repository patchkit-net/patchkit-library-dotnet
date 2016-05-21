using System;
using JetBrains.Annotations;

namespace PatchKit.API.Async
{
    /// <summary>
    /// Cancellation token for asynchronus operations.
    /// </summary>
    public struct AsyncCancellationToken
    {
        [CanBeNull]
        private readonly AsyncCancellationTokenSource _cancellationTokenSource;

        internal AsyncCancellationToken([NotNull] AsyncCancellationTokenSource cancellationTokenSource) : this()
        {
            if (cancellationTokenSource == null)
            {
                throw new ArgumentNullException("cancellationTokenSource");
            }

            _cancellationTokenSource = cancellationTokenSource;
        }

        public bool IsCancellationRequested
        {
            get { return _cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested; }
        }

        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }

        public AsyncCancellationTokenRegistration Register([NotNull] Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (_cancellationTokenSource == null)
            {
                return new AsyncCancellationTokenRegistration(null);
            }

            return _cancellationTokenSource.Register(callback);
        }
    }
}
