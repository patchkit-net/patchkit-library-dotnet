namespace PatchKit
{
	public partial class Api
	{
		/// <summary>
		/// Interface providing string downloading functionality.
		/// </summary>
		public interface IStringDownloader
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
			StringDownloadResult EndDownloadString(ICancellableAsyncResult asyncResult);
		}
	}
}