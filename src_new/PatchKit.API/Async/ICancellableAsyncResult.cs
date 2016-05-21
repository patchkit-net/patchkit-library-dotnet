using System;

namespace PatchKit.API.Async
{
    public interface ICancellableAsyncResult : IAsyncResult
    {
        bool IsCancelled { get; }

        bool Cancel();
    }
}
