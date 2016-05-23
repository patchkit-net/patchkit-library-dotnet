using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PatchKit.API.Async
{
    /// <summary>
    /// Cancellation token source for asynchronus operations.
    /// </summary>
    public sealed class AsyncCancellationTokenSource
    {
        private LinkedList<AsyncCancellationTokenRegistration> _registeredCallbacks;

        /// <summary>
        /// Cancellation token.
        /// </summary>
        public AsyncCancellationToken Token
        {
            get { return new AsyncCancellationToken(this); }
        }

        /// <summary>
        /// <c>True</c> if cancellation is requested. Otherwise <c>false</c>.
        /// </summary>
        public bool IsCancellationRequested { get; private set; }

        /// <summary>
        /// Requests the cancellation. Executes all of callbacks registered with <see cref="AsyncCancellationToken.Register"/>.
        /// </summary>
        public void Cancel()
        {
            IsCancellationRequested = true;

            if (_registeredCallbacks != null)
            {
                foreach (var registeredCallback in _registeredCallbacks)
                {
                    if (registeredCallback.Callback != null)
                    {
                        registeredCallback.Callback();
                    }
                }
            }
        }

        internal AsyncCancellationTokenRegistration Register([NotNull] Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            var registration = new AsyncCancellationTokenRegistration(callback);

            if (IsCancellationRequested)
            {
                callback();
            }

            if (_registeredCallbacks == null)
            {
                _registeredCallbacks = new LinkedList<AsyncCancellationTokenRegistration>();
            }

            _registeredCallbacks.AddLast(registration);

            return registration;
        }
    }
}
