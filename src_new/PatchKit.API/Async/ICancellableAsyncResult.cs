using System;

namespace PatchKit.API.Async
{
    public interface ICancellableAsyncResult : IAsyncResult
    {
        bool HasBeenCancelled { get; }

        void Cancel();
    }
}
