using System;
using PatchKit.API.Data;
using PatchKit.API.Async;
namespace PatchKit.API
{
    public partial class PatchKitAPI
    {
		/// <summary>Gets basic information of all published versions.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppVersionsList(string secretKey, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppVersion[]>(string.Format("1/apps/{0}/versions", secretKey), callback, state);
		}
		
		public AppVersion[] EndGetAppVersionsList(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppVersion[]>(asyncResult);
		}

		#if NET45
		/// <summary>Gets basic information of all published versions.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppVersion[]> GetAppVersionsListAsync(string secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppVersionsList(secretKey, callback), EndGetAppVersionsList, cancellationToken);
		}
		#endif

		/// <summary>Gets basic information of all published versions.</summary>
		/// <param name="secretKey">Application secret.</param>
		public AppVersion[] GetAppVersionsList(string secretKey)
		{
			return EndGetAppVersionsList(BeginGetAppVersionsList(secretKey));
		}

		/// <summary>Gets the latest published version information of given application.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppLatestVersion(string secretKey, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppVersion>(string.Format("1/apps/{0}/versions/latest", secretKey), callback, state);
		}
		
		public AppVersion EndGetAppLatestVersion(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppVersion>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the latest published version information of given application.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppVersion> GetAppLatestVersionAsync(string secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppLatestVersion(secretKey, callback), EndGetAppLatestVersion, cancellationToken);
		}
		#endif

		/// <summary>Gets the latest published version information of given application.</summary>
		/// <param name="secretKey">Application secret.</param>
		public AppVersion GetAppLatestVersion(string secretKey)
		{
			return EndGetAppLatestVersion(BeginGetAppLatestVersion(secretKey));
		}

		/// <summary>Gets the latest published version number of given application.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppLatestVersionId(string secretKey, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppLatestVersionId>(string.Format("1/apps/{0}/versions/latest/id", secretKey), callback, state);
		}
		
		public AppLatestVersionId EndGetAppLatestVersionId(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppLatestVersionId>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the latest published version number of given application.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppLatestVersionId> GetAppLatestVersionIdAsync(string secretKey, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppLatestVersionId(secretKey, callback), EndGetAppLatestVersionId, cancellationToken);
		}
		#endif

		/// <summary>Gets the latest published version number of given application.</summary>
		/// <param name="secretKey">Application secret.</param>
		public AppLatestVersionId GetAppLatestVersionId(string secretKey)
		{
			return EndGetAppLatestVersionId(BeginGetAppLatestVersionId(secretKey));
		}

		/// <summary>Gets basic information for given version id.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppVersion(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppVersion>(string.Format("1/apps/{0}/versions/{1}", secretKey, version), callback, state);
		}
		
		public AppVersion EndGetAppVersion(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppVersion>(asyncResult);
		}

		#if NET45
		/// <summary>Gets basic information for given version id.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppVersion> GetAppVersionAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppVersion(secretKey, version, callback), EndGetAppVersion, cancellationToken);
		}
		#endif

		/// <summary>Gets basic information for given version id.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		public AppVersion GetAppVersion(string secretKey, int version)
		{
			return EndGetAppVersion(BeginGetAppVersion(secretKey, version));
		}

		/// <summary>Gets the content summary of given version.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppContentSummary(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppContentSummary>(string.Format("1/apps/{0}/versions/{1}/content_summary", secretKey, version), callback, state);
		}
		
		public AppContentSummary EndGetAppContentSummary(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppContentSummary>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the content summary of given version.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppContentSummary> GetAppContentSummaryAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppContentSummary(secretKey, version, callback), EndGetAppContentSummary, cancellationToken);
		}
		#endif

		/// <summary>Gets the content summary of given version.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		public AppContentSummary GetAppContentSummary(string secretKey, int version)
		{
			return EndGetAppContentSummary(BeginGetAppContentSummary(secretKey, version));
		}

		/// <summary>Gets the diff summary of given version.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppDiffSummary(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppDiffSummary>(string.Format("1/apps/{0}/versions/{1}/diff_summary", secretKey, version), callback, state);
		}
		
		public AppDiffSummary EndGetAppDiffSummary(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppDiffSummary>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the diff summary of given version.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppDiffSummary> GetAppDiffSummaryAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppDiffSummary(secretKey, version, callback), EndGetAppDiffSummary, cancellationToken);
		}
		#endif

		/// <summary>Gets the diff summary of given version.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		public AppDiffSummary GetAppDiffSummary(string secretKey, int version)
		{
			return EndGetAppDiffSummary(BeginGetAppDiffSummary(secretKey, version));
		}

		/// <summary>Gets the URL to content torrent file.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppContentTorrentUrl(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppContentTorrentUrl>(string.Format("1/apps/{0}/versions/{1}/content_torrent_url", secretKey, version), callback, state);
		}
		
		public AppContentTorrentUrl EndGetAppContentTorrentUrl(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppContentTorrentUrl>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the URL to content torrent file.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppContentTorrentUrl> GetAppContentTorrentUrlAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppContentTorrentUrl(secretKey, version, callback), EndGetAppContentTorrentUrl, cancellationToken);
		}
		#endif

		/// <summary>Gets the URL to content torrent file.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		public AppContentTorrentUrl GetAppContentTorrentUrl(string secretKey, int version)
		{
			return EndGetAppContentTorrentUrl(BeginGetAppContentTorrentUrl(secretKey, version));
		}

		/// <summary>Gets the URL to diff torrent file.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppDiffTorrentUrl(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppDiffTorrentUrl>(string.Format("1/apps/{0}/versions/{1}/diff_torrent_url", secretKey, version), callback, state);
		}
		
		public AppDiffTorrentUrl EndGetAppDiffTorrentUrl(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppDiffTorrentUrl>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the URL to diff torrent file.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppDiffTorrentUrl> GetAppDiffTorrentUrlAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppDiffTorrentUrl(secretKey, version, callback), EndGetAppDiffTorrentUrl, cancellationToken);
		}
		#endif

		/// <summary>Gets the URL to diff torrent file.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		public AppDiffTorrentUrl GetAppDiffTorrentUrl(string secretKey, int version)
		{
			return EndGetAppDiffTorrentUrl(BeginGetAppDiffTorrentUrl(secretKey, version));
		}

		/// <summary>Gets the URLs directly to content file distributed on all online p2p nodes. Usually you will need only one to get the data. All of these URLs are pointing to the same file distributed on different nodes. Nodes recognized as offline are not listed.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppContentUrls(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppContentUrl[]>(string.Format("1/apps/{0}/versions/{1}/content_urls", secretKey, version), callback, state);
		}
		
		public AppContentUrl[] EndGetAppContentUrls(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppContentUrl[]>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the URLs directly to content file distributed on all online p2p nodes. Usually you will need only one to get the data. All of these URLs are pointing to the same file distributed on different nodes. Nodes recognized as offline are not listed.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppContentUrl[]> GetAppContentUrlsAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppContentUrls(secretKey, version, callback), EndGetAppContentUrls, cancellationToken);
		}
		#endif

		/// <summary>Gets the URLs directly to content file distributed on all online p2p nodes. Usually you will need only one to get the data. All of these URLs are pointing to the same file distributed on different nodes. Nodes recognized as offline are not listed.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		public AppContentUrl[] GetAppContentUrls(string secretKey, int version)
		{
			return EndGetAppContentUrls(BeginGetAppContentUrls(secretKey, version));
		}

		/// <summary>Gets the URLs directly to diff file distributed on all online p2p nodes. Usually you will need only one to get the data. All of these URLs are pointing to the same file distributed on different nodes. Nodes recognized as offline are not listed.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="callback">The <see cref="CancellableAsyncCallback" /> delegate.</param>
        /// <param name="state">The state object for this request.</param>
		public ICancellableAsyncResult BeginGetAppDiffUrls(string secretKey, int version, CancellableAsyncCallback callback = null, object state = null)
		{
			return BeginAPIRequest<AppDiffUrl[]>(string.Format("1/apps/{0}/versions/{1}/diff_urls", secretKey, version), callback, state);
		}
		
		public AppDiffUrl[] EndGetAppDiffUrls(IAsyncResult asyncResult)
		{
			return EndAPIRequest<AppDiffUrl[]>(asyncResult);
		}

		#if NET45
		/// <summary>Gets the URLs directly to diff file distributed on all online p2p nodes. Usually you will need only one to get the data. All of these URLs are pointing to the same file distributed on different nodes. Nodes recognized as offline are not listed.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public System.Threading.Tasks.Task<AppDiffUrl[]> GetAppDiffUrlsAsync(string secretKey, int version, System.Threading.CancellationToken cancellationToken)
		{
			return ToAsync(callback => BeginGetAppDiffUrls(secretKey, version, callback), EndGetAppDiffUrls, cancellationToken);
		}
		#endif

		/// <summary>Gets the URLs directly to diff file distributed on all online p2p nodes. Usually you will need only one to get the data. All of these URLs are pointing to the same file distributed on different nodes. Nodes recognized as offline are not listed.</summary>
		/// <param name="secretKey">Application secret.</param>
		/// <param name="version">Application version id.</param>
		public AppDiffUrl[] GetAppDiffUrls(string secretKey, int version)
		{
			return EndGetAppDiffUrls(BeginGetAppDiffUrls(secretKey, version));
		}

		
	}
}