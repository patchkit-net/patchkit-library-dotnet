using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using PatchKit.API.Data;
using PatchKit.API.Web;

namespace PatchKit.API
{
    /// <summary>
    /// PatchKit API provider.
    /// </summary>
    public class PatchKitAPI
    {
        private readonly IWWW _www;

        private readonly PatchKitAPISettings _settings;

        public PatchKitAPI(PatchKitAPISettings settings, IWWW www)
        {
            if (www == null)
            {
                throw new ArgumentNullException("www");
            }

            _settings = settings;
            _www = www;
        }

        public PatchKitAPI(PatchKitAPISettings settings) : this(settings, new DefaultWWW())
        {
        }

        public PatchKitAPIAsyncResult<AppVersion[]> GetAppVersionsList()
        {
            string methodUrl = string.Format("1/apps/{0}/versions", _settings.SecretKey);

            return APIRequest<AppVersion[]>(methodUrl);
        }

        public PatchKitAPIAsyncResult<AppVersion> GetAppLatestVersion()
        {
            string methodUrl = string.Format("1/apps/{0}/versions/latest", _settings.SecretKey);

			return APIRequest<AppVersion> (methodUrl);
        }

        public PatchKitAPIAsyncResult<AppLatestVersionID> GetAppLatestVersionID()
        {
            string methodUrl = string.Format("1/apps/{0}/versions/latest/id", _settings.SecretKey);

            return APIRequest<AppLatestVersionID>(methodUrl);
        }

        public PatchKitAPIAsyncResult<AppVersion> GetAppVersion(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}", _settings.SecretKey, version);

			return APIRequest<AppVersion> (methodUrl);
		}

		public PatchKitAPIAsyncResult<AppContentSummary> GetAppContentSummary(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/content_summary", _settings.SecretKey, version);

			return APIRequest<AppContentSummary> (methodUrl);
		}

		public PatchKitAPIAsyncResult<AppContentTorrentUrl> GetAppContentTorrentUrl(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/content_torrent_url", _settings.SecretKey, version);

			return APIRequest<AppContentTorrentUrl> (methodUrl);
		}

        public PatchKitAPIAsyncResult<AppContentUrl[]> GetAppContentUrls(int version)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/content_urls", _settings.SecretKey, version);

            return APIRequest<AppContentUrl[]>(methodUrl);
        }

        public PatchKitAPIAsyncResult<AppDiffSummary> GetAppDiffSummary(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/diff_summary", _settings.SecretKey, version);

			return APIRequest<AppDiffSummary> (methodUrl);
		}

		public PatchKitAPIAsyncResult<AppDiffTorrentUrl> GetAppDiffTorrentUrl(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/diff_torrent_url", _settings.SecretKey, version);

			return APIRequest<AppDiffTorrentUrl> (methodUrl);
		}

        public PatchKitAPIAsyncResult<AppDiffUrl[]> GetAppDiffUrls(int version)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/diff_urls", _settings.SecretKey, version);

            return APIRequest<AppDiffUrl[]>(methodUrl);
        }

        public T EndAPIRequest<T>(IAsyncResult asyncResult)
        {
            var patchKitAPIAsyncResult = asyncResult as PatchKitAPIAsyncResult<T>;

            if (patchKitAPIAsyncResult == null)
            {
                throw new ArgumentException("asyncResult");
            }

            patchKitAPIAsyncResult.AsyncWaitHandle.WaitOne();

            if (patchKitAPIAsyncResult.Exception != null)
            {
                throw patchKitAPIAsyncResult.Exception;
            }

            return patchKitAPIAsyncResult.Result;
        }

        private PatchKitAPIAsyncResult<T> APIRequest<T>(string url)
		{
			var result = new PatchKitAPIAsyncResult<T> (cancellationToken => DownloadAndVerifyServerResponse<T>(url, cancellationToken), null, null);

			return result;
		}

        private T DownloadAndVerifyServerResponse<T>(string requestUrl, PatchKitAPICancellationToken cancellationToken)
        {
            var requests = new PatchKitAPIAsyncResult<WWWResponse<string>>[_settings.MirrorAPIUrls.Length + 1];

            PatchKitAPIAsyncResult<WWWResponse<string>> correctResult = null;

            try
            {
                int i = 0;

                Stopwatch watch = new Stopwatch();
                watch.Start();

                while (correctResult == null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    correctResult = FindCorrectRequest(requests);

                    if (requests.All(r => r != null && r.IsCompleted) && correctResult == null)
                    {
                        throw requests[0].Exception ?? new PatchKitAPIException("Unexcepted API response.", requests[0].Result.StatusCode);
                    }

                    if (i < _settings.MirrorAPIUrls.Length + 1)
                    {
                        if (requests.All(r => r == null || r.IsCompleted) || watch.ElapsedMilliseconds >= _settings.DelayBetweenMirrorRequests)
                        {
                            string url = i == 0 ? _settings.APIUrl : _settings.MirrorAPIUrls[i - 1];

                            string fullUrl = requestUrl.EndsWith("/") ? url + requestUrl : url + "/" + requestUrl;

                            requests[i] = new PatchKitAPIAsyncResult<WWWResponse<string>>(cancellationToken2 => _www.DownloadString(fullUrl, cancellationToken2), null, null);

                            watch.Reset();
                            watch.Start();

                            i++;
                        }
                    }

                    Thread.Sleep(1);
                }

                if (correctResult.Result.StatusCode == 200)
                {
                    return JsonConvert.DeserializeObject<T>(correctResult.Result.Value);
                }

                throw new PatchKitAPIException("Unexcepted API response.", correctResult.Result.StatusCode);
            }
            finally
            {
                foreach (var r in requests)
                {
                    if (r != null && r != correctResult)
                    {
                        r.Cancel();
                        r.AsyncWaitHandle.WaitOne();
                    }
                }
            }
        }

        private static PatchKitAPIAsyncResult<WWWResponse<string>> FindCorrectRequest(IList<PatchKitAPIAsyncResult<WWWResponse<string>>> requests)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                var r = requests[i];
                if (r != null && r.IsCompleted && !r.HasBeenCancelled && r.Exception == null)
                {
                    if (i == 0)
                    {
                        if (r.Result.StatusCode == 200 || r.Result.StatusCode == 400 || r.Result.StatusCode == 401 || r.Result.StatusCode == 404)
                        {
                            return r;
                        }
                    }
                    else
                    {
                        if (r.Result.StatusCode == 200)
                        {
                            return r;
                        }
                    }
                }
            }

            return null;
        }
    }
}
