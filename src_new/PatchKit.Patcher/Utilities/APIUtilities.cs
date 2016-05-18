using System.Threading;
using PatchKit.API;

namespace PatchKit.Patcher.Utilities
{
    internal static class APIUtilities
    {
        public static T Wait<T>(this PatchKitAPIAsyncResult<T> @this)
        {
            while (!@this.IsCompleted)
            {
                Thread.Sleep(1);
            }

            if (@this.Exception != null)
            {
                throw @this.Exception;
            }

            return @this.Result;
        }

        public static T Wait<T>(this PatchKitAPIAsyncResult<T> @this, CancellationToken cancellationToken)
        {
            while (!@this.IsCompleted)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(1);
            }

            if (@this.Exception != null)
            {
                throw @this.Exception;
            }

            return @this.Result;
        }
    }
}
