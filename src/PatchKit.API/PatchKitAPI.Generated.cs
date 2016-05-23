using System;
using PatchKit.API.Data;
using PatchKit.API.Async;
namespace PatchKit.API
{
    public partial class PatchKitAPI
    {
		public ICancellableAsyncResult BeginGetAppVersionsList(string secretKey, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppVersion[]>(string.Format("1/apps/{0}/versions", secretKey), callback, state);
		}
		
		public AppVersion[] EndGetAppVersionsList(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppVersion[]>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppVersion[]> GetAppVersionsListAsync(string secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppVersionsList(secretKey, callback), EndGetAppVersionsList, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppLatestVersion(string secretKey, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppVersion>(string.Format("1/apps/{0}/versions/latest", secretKey), callback, state);
		}
		
		public AppVersion EndGetAppLatestVersion(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppVersion>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppVersion> GetAppLatestVersionAsync(string secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppLatestVersion(secretKey, callback), EndGetAppLatestVersion, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppLatestVersionId(string secretKey, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppLatestVersionId>(string.Format("1/apps/{0}/versions/latest/id", secretKey), callback, state);
		}
		
		public AppLatestVersionId EndGetAppLatestVersionId(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppLatestVersionId>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppLatestVersionId> GetAppLatestVersionIdAsync(string secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppLatestVersionId(secretKey, callback), EndGetAppLatestVersionId, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppVersion(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppVersion>(string.Format("1/apps/{0}/versions/{1}", secretKey, version), callback, state);
		}
		
		public AppVersion EndGetAppVersion(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppVersion>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppVersion> GetAppVersionAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppVersion(secretKey, version, callback), EndGetAppVersion, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppContentSummary(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppContentSummary>(string.Format("1/apps/{0}/versions/{1}/content_summary", secretKey, version), callback, state);
		}
		
		public AppContentSummary EndGetAppContentSummary(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppContentSummary>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppContentSummary> GetAppContentSummaryAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppContentSummary(secretKey, version, callback), EndGetAppContentSummary, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppContentTorrentUrl(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppContentTorrentUrl>(string.Format("1/apps/{0}/versions/{1}/content_torrent_url", secretKey, version), callback, state);
		}
		
		public AppContentTorrentUrl EndGetAppContentTorrentUrl(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppContentTorrentUrl>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppContentTorrentUrl> GetAppContentTorrentUrlAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppContentTorrentUrl(secretKey, version, callback), EndGetAppContentTorrentUrl, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppContentUrls(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppContentUrl[]>(string.Format("1/apps/{0}/versions/{1}/content_urls", secretKey, version), callback, state);
		}
		
		public AppContentUrl[] EndGetAppContentUrls(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppContentUrl[]>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppContentUrl[]> GetAppContentUrlsAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppContentUrls(secretKey, version, callback), EndGetAppContentUrls, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppDiffSummary(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppDiffSummary>(string.Format("1/apps/{0}/versions/{1}/diff_summary", secretKey, version), callback, state);
		}
		
		public AppDiffSummary EndGetAppDiffSummary(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppDiffSummary>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppDiffSummary> GetAppDiffSummaryAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppDiffSummary(secretKey, version, callback), EndGetAppDiffSummary, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppDiffTorrentUrl(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppDiffTorrentUrl>(string.Format("1/apps/{0}/versions/{1}/diff_torrent_url", secretKey, version), callback, state);
		}
		
		public AppDiffTorrentUrl EndGetAppDiffTorrentUrl(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppDiffTorrentUrl>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppDiffTorrentUrl> GetAppDiffTorrentUrlAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppDiffTorrentUrl(secretKey, version, callback), EndGetAppDiffTorrentUrl, cancellationToken);
		}
		#endif

		public ICancellableAsyncResult BeginGetAppDiffUrls(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppDiffUrl[]>(string.Format("1/apps/{0}/versions/{1}/diff_urls", secretKey, version), callback, state);
		}
		
		public AppDiffUrl[] EndGetAppDiffUrls(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppDiffUrl[]>(asyncResult);
		}

		#if NET45
		public System.Threading.Tasks.Task<AppDiffUrl[]> GetAppDiffUrlsAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppDiffUrls(secretKey, version, callback), EndGetAppDiffUrls, cancellationToken);
		}
		#endif

		
	}
}