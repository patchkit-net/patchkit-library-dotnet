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
        private LinkedList<Action> _registeredCallbacks;

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
                    registeredCallback();
                }
            }
        }

        internal void Register([NotNull] Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            if (IsCancellationRequested)
            {
                callback();
                return;
            }

            if (_registeredCallbacks == null)
            {
                _registeredCallbacks = new LinkedList<Action>();
            }

            _registeredCallbacks.AddLast(callback);
        }
    }
}
