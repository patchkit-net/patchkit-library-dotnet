namespace PatchKit
{
	/// <summary>
	/// Downloading API data through HTTP.
	/// </summary>
	public interface IApiHttpDownloader
	{
        /// <summary>
        /// Begins an asynchronus download of text string from <paramref name="url"/>.
        /// </summary>
        /// <param name="url">Url of resource.</param>
        /// <param name="timeout">Time after which request is aborted (<exception cref="System.TimeoutException"/> should be thrown after getting result).</param>
        /// <param name="asyncCallback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <returns>An <see cref="ICancellableAsyncResult"/> that references the asynchronus operation.</returns>
        ICancellableAsyncResult BeginDownloadString(string url, int timeout, CancellableAsyncCallback asyncCallback = null);

		/// <summary>
		/// Waits until the async operation is completed and retrieves result from it.
		/// </summary>
		/// <seealso cref="AsyncResultExtensions.FetchResultsFromAsyncOperation{T}"/>
		ApiHttpResult EndDownloadString(ICancellableAsyncResult asyncResult);
	}
}