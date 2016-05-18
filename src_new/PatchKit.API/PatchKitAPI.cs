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

        private PatchKitAPIAsyncResult<T> APIRequest<T>(string url)
		{
			var result = new PatchKitAPIAsyncResult<T> ();

		    ThreadPool.QueueUserWorkItem(state => DownloadAndVerifyServerResponse(url, result));

			return result;
		}

        private void DownloadAndVerifyServerResponse<T>(string requestUrl, PatchKitAPIAsyncResult<T> asyncResult)
        {
            var requests = new WWWRequest<string>[_settings.MirrorAPIUrls.Length + 1];

            WWWRequest<string> correctRequest = null;

            int i = 0;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (correctRequest == null && !asyncResult.IsCancelled)
            {
                correctRequest = FindCorrectRequest(requests);

                if (requests.All(r => r != null && (r.IsCompleted || r.Error != null)) && correctRequest == null)
                {
                    asyncResult.Complete(default(T), requests[0].Error ?? new PatchKitAPIException("API request failure.", requests[0].StatusCode));

                    break;
                }

                if (i < _settings.MirrorAPIUrls.Length + 1)
                {
                    if (requests.All(r => r == null || r.IsCompleted || r.Error != null) || watch.ElapsedMilliseconds >= _settings.DelayBetweenMirrorRequests)
                    {
                        string url = i == 0 ? _settings.APIUrl : _settings.MirrorAPIUrls[i - 1];

                        requests[i] = new WWWRequest<string>(requestUrl.EndsWith("/") ? url + requestUrl : url + "/" + requestUrl);

                        _www.DownloadString(requests[i]);

                        watch.Reset();
                        watch.Start();

                        i++;
                    }
                }

                Thread.Sleep(1);
            }

            foreach (var r in requests)
            {
                if (r != null && r != correctRequest)
                {
                    r.IsCancelled = true;
                }
            }

            if (correctRequest != null)
            {
                if (correctRequest.StatusCode == 200)
                {
                    try
                    {
                        T value = JsonConvert.DeserializeObject<T>(correctRequest.Value);

                        asyncResult.Complete(value, null);
                    }
                    catch (Exception exception)
                    {
                        asyncResult.Complete(default(T), exception);
                    }
                }
                else
                {
                    asyncResult.Complete(default(T), new PatchKitAPIException("API error.", correctRequest.StatusCode));
                }
            }
        }

        private static WWWRequest<string> FindCorrectRequest(IList<WWWRequest<string>> requests)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                var r = requests[i];
                if (r != null && r.IsCompleted && r.Error == null)
                {
                    if (i == 0)
                    {
                        if (r.StatusCode == 200 || r.StatusCode == 400 || r.StatusCode == 401 || r.StatusCode == 404)
                        {
                            return r;
                        }
                    }
                    else
                    {
                        if (r.StatusCode == 200)
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
