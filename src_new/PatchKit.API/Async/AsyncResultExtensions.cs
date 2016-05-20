using System;
using JetBrains.Annotations;

namespace PatchKit.API.Async
{
    public static class AsyncResultExtensions
    {
        public static T FetchResultsFromAsyncOperation<T>([NotNull] this AsyncResult<T> @this)
        {
            @this.AsyncWaitHandle.WaitOne();

            if (@this.Exception != null)
            {
                throw new Exception("Async operation exception.", @this.Exception);
            }

            if (@this.HasBeenCancelled)
            {
                throw new InvalidOperationException("Async operation has been cancelled during result fetching. You should never try to retrive result after cancelling the operation.");
            }

            return @this.Result;
        }
    }
}
