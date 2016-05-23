namespace PatchKit.API.NET45
{
    /// <summary>
    /// .NET 4.5 extensions for <see cref="PatchKitAPI" />.
    /// </summary>
    public static class PatchKitAPIExtensions
    {
		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppVersion[]> GetAppVersionsListAsync(this PatchKitAPI @this, System.String secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppVersionsList(secretKey, callback), @this.EndGetAppVersionsList, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppVersion> GetAppLatestVersionAsync(this PatchKitAPI @this, System.String secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppLatestVersion(secretKey, callback), @this.EndGetAppLatestVersion, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppLatestVersionId> GetAppLatestVersionIdAsync(this PatchKitAPI @this, System.String secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppLatestVersionId(secretKey, callback), @this.EndGetAppLatestVersionId, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppVersion> GetAppVersionAsync(this PatchKitAPI @this, System.String secretKey, System.Int32 version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppVersion(secretKey, version, callback), @this.EndGetAppVersion, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppContentSummary> GetAppContentSummaryAsync(this PatchKitAPI @this, System.String secretKey, System.Int32 version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppContentSummary(secretKey, version, callback), @this.EndGetAppContentSummary, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppContentTorrentUrl> GetAppContentTorrentUrlAsync(this PatchKitAPI @this, System.String secretKey, System.Int32 version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppContentTorrentUrl(secretKey, version, callback), @this.EndGetAppContentTorrentUrl, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppContentUrl[]> GetAppContentUrlsAsync(this PatchKitAPI @this, System.String secretKey, System.Int32 version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppContentUrls(secretKey, version, callback), @this.EndGetAppContentUrls, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppDiffSummary> GetAppDiffSummaryAsync(this PatchKitAPI @this, System.String secretKey, System.Int32 version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppDiffSummary(secretKey, version, callback), @this.EndGetAppDiffSummary, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppDiffTorrentUrl> GetAppDiffTorrentUrlAsync(this PatchKitAPI @this, System.String secretKey, System.Int32 version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppDiffTorrentUrl(secretKey, version, callback), @this.EndGetAppDiffTorrentUrl, cancellationToken);
		}

		public static System.Threading.Tasks.Task<PatchKit.API.Data.AppDiffUrl[]> GetAppDiffUrlsAsync(this PatchKitAPI @this, System.String secretKey, System.Int32 version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => @this.BeginGetAppDiffUrls(secretKey, version, callback), @this.EndGetAppDiffUrls, cancellationToken);
		}

		
		private static System.Threading.Tasks.Task<T> ToAsync<T>(System.Func<PatchKit.API.Async.CancellableAsyncCallback, PatchKit.API.Async.ICancellableAsyncResult> beginMethod,
            System.Func<System.IAsyncResult, T> endMethod, System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.Factory.FromAsync((callback, o) =>
            {
                var asyncResult = beginMethod(ar => callback(ar));

                cancellationToken.Register(() => asyncResult.Cancel());

                return asyncResult;
            }, endMethod, null);
        }
	}
}