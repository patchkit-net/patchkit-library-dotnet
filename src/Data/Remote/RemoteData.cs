using System;
using System.Threading;
using Newtonsoft.Json;
using PatchKit.Downloader;
using PatchKit.Utilities;

namespace PatchKit.Data.Remote
{
    /// <summary>
    /// Remote data.
    /// </summary>
    internal class RemoteData
    {
        private readonly PatcherSettings _connectionSettings;
        private readonly IDownloader _downloader;

        private int? _currentVersion;

        public RemoteData(PatcherSettings connectionSettings, IDownloader downloader)
        {
            _connectionSettings = connectionSettings;
            _downloader = downloader;
        }

        private RemoteResponse<T> ParseAndVerifyServerResponse<T>(string responseText)
        {
            RemoteResponse<T> response = JsonConvert.DeserializeObject<RemoteResponse<T>>(responseText);

            if (response.Status != 0)
            {
                throw new PatcherServerException(response.StatusMessage, response.Status);
            }

            return response;
        }

        /// <summary>
        /// Returns currently available version of the application.
        /// </summary>
        public int GetCurrentVersion(CancellationToken cancelToken)
        {
            // caching current version to not ask for it twice
            if (!_currentVersion.HasValue)
            {
                string methodUrl = string.Format("1/apps/{0}/versions/latest", _connectionSettings.SecretKey);

                string content = _downloader.DownloadString(
                    new Uri(_connectionSettings.ServiceURL, methodUrl),
                    progress => { }, cancelToken);

                var data = ParseAndVerifyServerResponse<RemoteCurrentVersionInfo>(content);
                _currentVersion = data.Data.Published;
            }

            return _currentVersion.Value;
        }

        /// <summary>
        /// Returns info about specified version.
        /// </summary>
        public RemoteVersionInfo GetVersionInfo(int version, CancellationToken cancelToken)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}", _connectionSettings.SecretKey, version);

            string summary = _downloader.DownloadString(
                new Uri(_connectionSettings.ServiceURL, methodUrl),
                progress => { }, cancelToken);

            var data = ParseAndVerifyServerResponse<RemoteVersionInfo>(summary);
            return data.Data;
        }

        /// <summary>
        /// Returns info about specified version.
        /// </summary>
        public RemoteAllVersionsInfo GetAllVersionsInfo(CancellationToken cancelToken)
        {
            string methodUrl = string.Format("1/apps/{0}/versions", _connectionSettings.SecretKey);

            string summary = _downloader.DownloadString(
                new Uri(_connectionSettings.ServiceURL, methodUrl),
                progress => { }, cancelToken);

            var data = ParseAndVerifyServerResponse<RemoteAllVersionsInfo>(summary);
            return data.Data;
        }

        /// <summary>
        /// Returns summary of content for specified version.
        /// </summary>
        public RemoteContentSummary GetContentSummary(int version, CancellationToken cancelToken)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/contentSummary", _connectionSettings.SecretKey, version);

            string summary = _downloader.DownloadString(
                new Uri(_connectionSettings.ServiceURL, methodUrl),
                progress => { }, cancelToken);

            var data = ParseAndVerifyServerResponse<RemoteContentSummary>(summary);
            return data.Data;
        }

        /// <summary>
        /// Returns summary of diff for specified version.
        /// </summary>
        public RemoteDiffSummary GetDiffSummary(int version, CancellationToken cancelToken)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/diffSummary", _connectionSettings.SecretKey, version);

            string summary = _downloader.DownloadString(
                new Uri(_connectionSettings.ServiceURL, methodUrl),
                progress => { }, cancelToken);

            var data = ParseAndVerifyServerResponse<RemoteDiffSummary>(summary);
            return data.Data;
        }

        public string DownloadContent(int version, DownloaderProgressHandler progress, CancellationToken cancellationToken)
        {
            Uri torrentUri = GetContentTorrentUri(version, cancellationToken);
            string contentPath = _downloader.DownloadTorrent(torrentUri, progress, cancellationToken);
            return contentPath;
        }

        public string DownloadDiff(int version, DownloaderProgressHandler progress, CancellationToken cancellationToken)
        {
            Uri torrentUri = GetDiffTorrentUri(version, cancellationToken);
            string diffPath = _downloader.DownloadTorrent(torrentUri, progress, cancellationToken);
            return diffPath;
        }

        private Uri GetContentTorrentUri(int version, CancellationToken cancellationToken)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/contentTorrentUrl", _connectionSettings.SecretKey, version);

            string info = _downloader.DownloadString(
                new Uri(_connectionSettings.ServiceURL, methodUrl),
                progress => { }, cancellationToken);

            var data = ParseAndVerifyServerResponse<RemoteUrlInfo>(info);
            return new Uri(data.Data.Url);
        }

        private Uri GetDiffTorrentUri(int version, CancellationToken cancellationToken)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/diffTorrentUrl", _connectionSettings.SecretKey, version);

            string info = _downloader.DownloadString(
                new Uri(_connectionSettings.ServiceURL, methodUrl),
                progress => { }, cancellationToken);

            var data = ParseAndVerifyServerResponse<RemoteUrlInfo>(info);
            return new Uri(data.Data.Url);
        }

        /// <summary>
        /// Returns URL to content package for specified version.
        /// </summary>
        public Uri GetContentPackageUrl(int version)
        {
            string relative = Urls.CombineRelative("download/content",
                "secret", _connectionSettings.SecretKey,
                "version", version);

            return new Uri(_connectionSettings.ServiceURL, relative);
        }

        /// <summary>
        /// Returns URL to diff package for specified version.
        /// </summary>
        public Uri GetDiffPackageUrl(int version)
        {
            string relative = Urls.CombineRelative("download/diff",
                "secret", _connectionSettings.SecretKey,
                "version", version);

            return new Uri(_connectionSettings.ServiceURL, relative);
        }

    }
}
