using PatchKit.API.Async;

namespace PatchKit.API.Web
{
    public interface IWWW
    {
        /// <summary>
        /// Begins an asynchronus download of text string from <paramref name="url"/>.
        /// </summary>
        /// <param name="url">Url.</param>
        /// <param name="asyncCallback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request. </param>
        /// <returns>An <see cref="ICancellableAsyncResult"/> that references the asynchronus operation.</returns>
        ICancellableAsyncResult BeginDownloadString(string url, CancellableAsyncCallback asyncCallback = null, object state = null);

        /// <summary>
        /// Waits until the async operation is completed and retrieves result from it.
        /// </summary>
        /// <seealso cref="AsyncResultExtensions.FetchResultsFromAsyncOperation{T}"/>
        WWWResponse<string> EndDownloadString(ICancellableAsyncResult asyncResult);
    }
}