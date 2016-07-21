using System;
using JetBrains.Annotations;

namespace PatchKit.Async
{
    public static class AsyncResultExtensions
    {
        /// <summary>
        /// Fetches the result from <see cref="AsyncResult{T}"/>.
        /// If <see cref="AsyncResult{T}.Exception"/> isn't <c>null</c> then it is thrown again as inner exception.
        /// If async operation has been cancelled then <see cref="OperationCanceledException"/> is thrown.
        /// </summary>
        /// <exception cref="Exception">
        /// <see cref="AsyncResult{T}.Exception"/> as inner exception.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// When async operation has been cancelled.
        /// </exception>
        /// <returns>Result retrieved from async result.</returns>
        public static T FetchResultsFromAsyncOperation<T>([NotNull] this AsyncResult<T> @this)
        {
            @this.AsyncWaitHandle.WaitOne();

            if (@this.Exception != null)
            {
                throw new Exception("Async operation exception.", @this.Exception);
            }

            if (@this.IsCancelled)
            {
                throw new OperationCanceledException();
            }

            return @this.Result;
        }
    }
}
