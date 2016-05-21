using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PatchKit.API.Async
{
    /// <summary>
    /// Cancellation token source for asynchronus operations.
    /// </summary>
    public class AsyncCancellationTokenSource
    {
        private LinkedList<AsyncCancellationTokenRegistration> _registeredCallbacks;

        public AsyncCancellationToken Token
        {
            get { return new AsyncCancellationToken(this); }
        }

        public bool IsCancellationRequested { get; protected set; }

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
