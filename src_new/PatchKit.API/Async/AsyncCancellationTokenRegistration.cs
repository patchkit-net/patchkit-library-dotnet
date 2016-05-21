using System;
using JetBrains.Annotations;

namespace PatchKit.API.Async
{
    /// <summary>
    /// Cancellation token callback registration. Callback is unregistered when this registration is disposed.
    /// </summary>
    public class AsyncCancellationTokenRegistration : IDisposable
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
