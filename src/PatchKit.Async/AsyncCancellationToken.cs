using System;
using JetBrains.Annotations;

namespace PatchKit
{
    /// <summary>
    /// Cancellation token for asynchronus operations.
    /// </summary>
    public struct AsyncCancellationToken
    {
        public static readonly AsyncCancellationToken None = new AsyncCancellationToken(null);

        [CanBeNull]
        private readonly AsyncCancellationTokenSource _cancellationTokenSource;

        internal AsyncCancellationToken(AsyncCancellationTokenSource cancellationTokenSource) : this()
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        /// <summary>
        /// <c>True</c> if cancellation is requested. Otherwise <c>false</c>.
        /// </summary>
        public bool IsCancellationRequested
        {
            get { return _cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested; }
        }

        /// <summary>
        /// Throws <see cref="OperationCanceledException"/> if cancellation is requested.
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// Registers a callback which executes on cancellation. If token is already cancelled, callback is executed immediately.
        /// </summary>
        /// <param name="callback">Callback to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="callback"/> is null.
        /// </exception>
        /// <returns>Registration which can be used to unregister the callback by disposing the registration object.</returns>
        /// <example>
        /// // Register cancellation callback
        /// var registration = cancellationToken.Register(() => Thread.CurrentThread.Abort);
        /// // Callback will be unregistered after leaving using scope.
        /// using(registration)
        /// {
        /// // Some operations
        /// }
        /// </example>
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
