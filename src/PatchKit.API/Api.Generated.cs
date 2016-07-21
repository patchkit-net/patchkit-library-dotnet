

using System;
using Newtonsoft.Json;
namespace PatchKit
{
    public partial class Api
    {
        public struct Job
        {
            /// <summary>Job GUID to be used with Jobs API.</summary>
            [JsonProperty("job_guid")]
            public string JobGuid;
        }
        
        public struct AppVersion
        {
            /// <summary>Initial version id.</summary>
            [JsonProperty("id")]
            public int Id;
            /// <summary>Version label.</summary>
            [JsonProperty("label")]
            public string Label;
            /// <summary>Description of changes.</summary>
            [JsonProperty("changelog")]
            public string Changelog;
            /// <summary>Unix timestamp of publish date.</summary>
            [JsonProperty("publish_date")]
            public long PublishDate;
            /// <summary>Guid of content file.</summary>
            [JsonProperty("content_guid")]
            public string ContentGuid;
            /// <summary>Guid of diff file or null if there's no diff.</summary>
            [JsonProperty("diff_guid")]
            public string DiffGuid;
            /// <summary>Set to true if this version is a draft version.</summary>
            [JsonProperty("draft")]
            public bool Draft;
        }
        
        public struct AppVersionId
        {
            /// <summary>Version id.</summary>
            [JsonProperty("id")]
            public int Id;
        }
        
        public struct AppContentSummary
        {
            /// <summary>Content size.</summary>
            [JsonProperty("size")]
            public long Size;
            /// <summary>Encryption method.</summary>
            [JsonProperty("encryption_method")]
            public string EncryptionMethod;
            /// <summary>Compression method.</summary>
            [JsonProperty("compression_method")]
            public string CompressionMethod;
            /// <summary></summary>
            [JsonProperty("files")]
            public AppContentSummaryFile[] Files;
        }
        
        public struct AppContentSummaryFile
        {
            /// <summary>File path.</summary>
            [JsonProperty("path")]
            public string Path;
            /// <summary>File hash.</summary>
            [JsonProperty("hash")]
            public string Hash;
        }
        
        public struct AppDiffSummary
        {
            /// <summary>Diff size.</summary>
            [JsonProperty("size")]
            public long Size;
            /// <summary>Encryption method.</summary>
            [JsonProperty("encryption_method")]
            public string EncryptionMethod;
            /// <summary>Compression method.</summary>
            [JsonProperty("compression_method")]
            public string CompressionMethod;
            /// <summary>List of added files.</summary>
            [JsonProperty("added_files")]
            public string[] AddedFiles;
            /// <summary>List of modified files.</summary>
            [JsonProperty("modified_files")]
            public string[] ModifiedFiles;
            /// <summary>List of removed files.</summary>
            [JsonProperty("removed_files")]
            public string[] RemovedFiles;
        }
        
        public struct AppContentTorrentUrl
        {
            /// <summary>Url to content torrent file.</summary>
            [JsonProperty("url")]
            public string Url;
        }
        
        public struct AppDiffTorrentUrl
        {
            /// <summary>Url to diff torrent file.</summary>
            [JsonProperty("url")]
            public string Url;
        }
        
        public struct AppContentUrl
        {
            /// <summary>Url to content file.</summary>
            [JsonProperty("url")]
            public string Url;
        }
        
        public struct AppDiffUrl
        {
            /// <summary>Url to diff file.</summary>
            [JsonProperty("url")]
            public string Url;
        }
        
        /// <summary>Gets a complete changelog of an application.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppChangelog(string appSecret, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/changelog";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            return BeginApiRequest<object[]>(resource + "?" + query, callback, state);
        }
        
        public object[] EndGetAppChangelog(IAsyncResult asyncResult)
        {
            return EndApiRequest<object[]>(asyncResult);
        }
        
        /// <summary>Gets the basic information for all published versions. When API Key is provided, draft version information is included if draft version exists.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="apiKey">Application owner API key.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersionList(string appSecret, string apiKey = null, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            if (apiKey != null) query += "api_key="+apiKey+"&";
            return BeginApiRequest<AppVersion[]>(resource + "?" + query, callback, state);
        }
        
        public AppVersion[] EndGetAppVersionList(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppVersion[]>(asyncResult);
        }
        
        /// <summary>Gets latest application version object.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppLatestAppVersion(string appSecret, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/latest";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            return BeginApiRequest<AppVersion>(resource + "?" + query, callback, state);
        }
        
        public AppVersion EndGetAppLatestAppVersion(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppVersion>(asyncResult);
        }
        
        /// <summary>Gets latest application version id.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppLatestAppVersionId(string appSecret, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/latest/id";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            return BeginApiRequest<AppVersionId>(resource + "?" + query, callback, state);
        }
        
        public AppVersionId EndGetAppLatestAppVersionId(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppVersionId>(asyncResult);
        }
        
        /// <summary>Gets selected version object. If API key is provided, can get the information about draft version.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="versionId">Version id.</param>
        /// <param name="apiKey">Application owner API key.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersion(string appSecret, int versionId, string apiKey = null, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/{version_id}";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            resource = resource.Replace("{version_id}", versionId.ToString());
            if (apiKey != null) query += "api_key="+apiKey+"&";
            return BeginApiRequest<AppVersion>(resource + "?" + query, callback, state);
        }
        
        public AppVersion EndGetAppVersion(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppVersion>(asyncResult);
        }
        
        /// <summary>Gets selected version content summary.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="versionId">Version id.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersionContentSummary(string appSecret, int versionId, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/{version_id}/content_summary";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            resource = resource.Replace("{version_id}", versionId.ToString());
            return BeginApiRequest<AppContentSummary>(resource + "?" + query, callback, state);
        }
        
        public AppContentSummary EndGetAppVersionContentSummary(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppContentSummary>(asyncResult);
        }
        
        /// <summary>Gets selected version diff summary.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="versionId">Version id.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersionDiffSummary(string appSecret, int versionId, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/{version_id}/diff_summary";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            resource = resource.Replace("{version_id}", versionId.ToString());
            return BeginApiRequest<AppDiffSummary>(resource + "?" + query, callback, state);
        }
        
        public AppDiffSummary EndGetAppVersionDiffSummary(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppDiffSummary>(asyncResult);
        }
        
        /// <summary>Gets selected application version content torrent url.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="versionId">Version id.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersionContentTorrentUrl(string appSecret, int versionId, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/{version_id}/content_torrent_url";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            resource = resource.Replace("{version_id}", versionId.ToString());
            return BeginApiRequest<AppContentTorrentUrl>(resource + "?" + query, callback, state);
        }
        
        public AppContentTorrentUrl EndGetAppVersionContentTorrentUrl(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppContentTorrentUrl>(asyncResult);
        }
        
        /// <summary>Gets selected application version diff torrent url.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="versionId">Version id.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersionDiffTorrentUrl(string appSecret, int versionId, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/{version_id}/diff_torrent_url";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            resource = resource.Replace("{version_id}", versionId.ToString());
            return BeginApiRequest<AppDiffTorrentUrl>(resource + "?" + query, callback, state);
        }
        
        public AppDiffTorrentUrl EndGetAppVersionDiffTorrentUrl(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppDiffTorrentUrl>(asyncResult);
        }
        
        /// <summary>Gets selected application version content urls.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="versionId">Version id.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersionContentUrls(string appSecret, int versionId, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/{version_id}/content_urls";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            resource = resource.Replace("{version_id}", versionId.ToString());
            return BeginApiRequest<AppContentUrl[]>(resource + "?" + query, callback, state);
        }
        
        public AppContentUrl[] EndGetAppVersionContentUrls(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppContentUrl[]>(asyncResult);
        }
        
        /// <summary>Gets selected application version diff urls.</summary>
        /// <param name="appSecret">Secret of an application.</param>
        /// <param name="versionId">Version id.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="state">Operation state.</param>
        public ICancellableAsyncResult BeginGetAppVersionDiffUrls(string appSecret, int versionId, CancellableAsyncCallback callback = null, object state = null)
        {
            string resource = "/1/apps/{app_secret}/versions/{version_id}/diff_urls";
            string query = string.Empty;
            resource = resource.Replace("{app_secret}", appSecret);
            resource = resource.Replace("{version_id}", versionId.ToString());
            return BeginApiRequest<AppDiffUrl[]>(resource + "?" + query, callback, state);
        }
        
        public AppDiffUrl[] EndGetAppVersionDiffUrls(IAsyncResult asyncResult)
        {
            return EndApiRequest<AppDiffUrl[]>(asyncResult);
        }
        
    }
}
