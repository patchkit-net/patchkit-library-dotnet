using System;
using JetBrains.Annotations;

namespace PatchKit.Async
{
    /// <summary>
    /// Cancellation token callback registration. Callback is unregistered when registration object is disposed.
    /// </summary>
    public sealed class AsyncCancellationTokenRegistration : IDisposable
    {
        [CanBeNull]
        internal Action Callback { get; private set; }

        internal AsyncCancellationTokenRegistration(Action callback)
        {
            Callback = callback;
        }

        public void Dispose()
        {
            Callback = null;
        }
    }
}
