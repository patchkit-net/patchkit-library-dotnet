using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PatchKit.API.Async;
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

        public PatchKitAPI([NotNull] PatchKitAPISettings settings, IWWW www)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
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

        public ICancellableAsyncResult BeginGetAppVersionsList()
        {
            string methodUrl = string.Format("1/apps/{0}/versions", _settings.SecretKey);

            return BeginAPIRequest<AppVersion[]>(methodUrl);
        }

        public AppVersion[] EndGetAppVersionsList(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppVersion[]>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppLatestVersion()
        {
            string methodUrl = string.Format("1/apps/{0}/versions/latest", _settings.SecretKey);

			return BeginAPIRequest<AppVersion> (methodUrl);
        }

        public AppVersion EndGetAppLatestVersion(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppVersion>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppLatestVersionID()
        {
            string methodUrl = string.Format("1/apps/{0}/versions/latest/id", _settings.SecretKey);

            return BeginAPIRequest<AppLatestVersionID>(methodUrl);
        }

        public AppLatestVersionID EndGetAppLatestVersionID(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppLatestVersionID>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppVersion(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}", _settings.SecretKey, version);

			return BeginAPIRequest<AppVersion> (methodUrl);
		}

        public AppVersion EndGetAppVersion(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppVersion>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppContentSummary(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/content_summary", _settings.SecretKey, version);

			return BeginAPIRequest<AppContentSummary> (methodUrl);
		}

        public AppContentSummary EndGetAppContentSummary(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppContentSummary>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppContentTorrentUrl(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/content_torrent_url", _settings.SecretKey, version);

			return BeginAPIRequest<AppContentTorrentUrl> (methodUrl);
		}

        public AppContentTorrentUrl EndGetAppContentTorrentUrl(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppContentTorrentUrl>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppContentUrls(int version)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/content_urls", _settings.SecretKey, version);

            return BeginAPIRequest<AppContentUrl[]>(methodUrl);
        }

        public AppContentUrl[] EndGetAppContentUrls(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppContentUrl[]>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppDiffSummary(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/diff_summary", _settings.SecretKey, version);

			return BeginAPIRequest<AppDiffSummary> (methodUrl);
		}

        public AppDiffSummary EndGetAppDiffSummary(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppDiffSummary>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppDiffTorrentUrl(int version)
		{
			string methodUrl = string.Format("1/apps/{0}/versions/{1}/diff_torrent_url", _settings.SecretKey, version);

			return BeginAPIRequest<AppDiffTorrentUrl> (methodUrl);
		}

        public AppDiffTorrentUrl EndGetAppDiffTorrentUrl(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppDiffTorrentUrl>(asyncResult);
        }


        public ICancellableAsyncResult BeginGetAppDiffUrls(int version)
        {
            string methodUrl = string.Format("1/apps/{0}/versions/{1}/diff_urls", _settings.SecretKey, version);

            return BeginAPIRequest<AppDiffUrl[]>(methodUrl);
        }

        public AppDiffUrl[] EndGetAppDiffUrls(IAsyncResult asyncResult)
        {
            return EndAPIRequest<AppDiffUrl[]>(asyncResult);
        }


        private ICancellableAsyncResult BeginAPIRequest<T>(string url)
		{
			var result = new AsyncResult<T> (cancellationToken => DownloadAndVerifyServerResponse<T>(url, cancellationToken), null, null);

			return result;
		}

        private T EndAPIRequest<T>(IAsyncResult asyncResult)
        {
            var result = asyncResult as AsyncResult<T>;

            if (result == null)
            {
                throw new ArgumentException("asyncResult");
            }

            return result.FetchResultsFromAsyncOperation();
        }

        private T DownloadAndVerifyServerResponse<T>(string methodUrl, AsyncCancellationToken cancellationToken)
        {
            // Create dictionary of mirror requests and responses.
            var mirrorRequests = new Dictionary<ICancellableAsyncResult, WWWResponse<string>?>();

            ICancellableAsyncResult mainRequest = null;

            WWWResponse<string>?[] mainResponse = {null};

            Exception[] mainException = {null};

            WWWResponse<string>? correctResponse = null;

            object requestsLock = new object();

            try
            {
                // Register cancellation callback which pulses the request lock in order to unfreeze main operation thread.
                using (cancellationToken.Register(() => Monitor.PulseAll(requestsLock)))
                {
                    // Begin with main request.
                    mainRequest = _www.BeginDownloadString(GetUrl(_settings.APIUrl, methodUrl),
                        ar =>
                        {
                            try
                            {
                                if (!ar.IsCancelled)
                                {
                                    lock (requestsLock)
                                    {
                                        // Save the main response.
                                        mainResponse[0] = _www.EndDownloadString(ar);
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                // Save the exception because it would be returned if all requests fail.
                                mainException[0] = exception;
                            }
                            finally
                            {
                                lock (requestsLock)
                                {
                                    // Pulse the lock if there are no pending requests left.
                                    if (GetNumberOfUncompletedRequests(ar, mirrorRequests.Keys) <= 0)
                                    {
                                        Monitor.PulseAll(requestsLock);
                                    }
                                }
                            }
                        });
                    

                    // Make a request for each mirror.
                    foreach (var mirrorUrl in _settings.MirrorAPIUrls)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        lock (requestsLock)
                        {
                            // Wait 5 seconds, until a request is finished or until operation is cancelled.
                            if (GetNumberOfUncompletedRequests(mainRequest, mirrorRequests.Keys) > 0)
                            {
                                Monitor.Wait(requestsLock, (int)_settings.DelayBetweenMirrorRequests);
                            }
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        lock (requestsLock)
                        {
                            correctResponse = FindCorrectResponse(mainResponse[0], mirrorRequests.Values);
                        }

                        // If correct response is found, leave the foreach.
                        if (correctResponse != null)
                        {
                            break;
                        }

                        lock (requestsLock)
                        {
                            // Make new mirror request.
                            mirrorRequests.Add(_www.BeginDownloadString(GetUrl(mirrorUrl, methodUrl), ar =>
                            {
                                try
                                {
                                    lock (requestsLock)
                                    {
                                        // Save mirror response.
                                        mirrorRequests[ar] = _www.EndDownloadString(ar);
                                    }
                                }
                                finally
                                {
                                    lock (requestsLock)
                                    {
                                        // Pulse the lock if there are no pending requests left. 
                                        if (GetNumberOfUncompletedRequests(mainRequest, mirrorRequests.Keys) <= 0)
                                        {
                                            Monitor.PulseAll(requestsLock);
                                        }
                                    }
                                }
                            }), null);
                        }
                    }

                    lock (requestsLock)
                    {
                        // Check whether there are uncompleted requests and correct response is still not found.
                        while (GetNumberOfUncompletedRequests(mainRequest, mirrorRequests.Keys) > 0 &&
                               correctResponse == null)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            lock (requestsLock)
                            {
                                correctResponse = FindCorrectResponse(mainResponse[0], mirrorRequests.Values);
                            }

                            // Wait until a request is finished or until operation is cancelled.
                            Monitor.Wait(requestsLock);
                        }
                    }

                    lock (requestsLock)
                    {
                        correctResponse = FindCorrectResponse(mainResponse[0], mirrorRequests.Values);
                    }

                    // If correct response hasn't arrived from any source.
                    if (correctResponse == null)
                    {
                        // Rethrow exception from main request (if exists).
                        if (mainException[0] != null)
                        {
                            throw mainException[0];
                        }

                        // Throw API exception containing status code from the response (if exists).
                        if (mainResponse[0] != null)
                        {
                            throw new PatchKitAPIException("Unexcepted API response." + mainResponse[0].Value.StatusCode, mainResponse[0].Value.StatusCode);
                        }
                    }
                    else
                    {
                        // Check whether response status code is correct.
                        if (correctResponse.Value.StatusCode == 200)
                        {
                            // Deserialize response content.
                            return JsonConvert.DeserializeObject<T>(correctResponse.Value.Value);
                        }
                        else
                        {
                            // Throw API exception containing status code from the response.
                            throw new PatchKitAPIException("Unexcepted API response." + correctResponse.Value.StatusCode, correctResponse.Value.StatusCode);
                        }
                    }

                    throw new Exception("Unable to get response from servers.");
                }
            }
            finally
            {
                if (mainRequest != null && !mainRequest.IsCompleted)
                {
                    mainRequest.Cancel();
                }

                foreach (var r in mirrorRequests)
                {
                    if (!r.Key.IsCompleted)
                    {
                        r.Key.Cancel();
                    }
                }
            }
        }

        private static int GetNumberOfUncompletedRequests(ICancellableAsyncResult mainRequest,
            Dictionary<ICancellableAsyncResult, WWWResponse<string>?>.KeyCollection mirrorRequests)
        {
            return mirrorRequests.Count(r => !r.IsCompleted) + (mainRequest.IsCompleted ? 0 : 1);
        }

        private static string GetUrl(string baseUrl, string methodUrl)
        {
            return baseUrl.EndsWith("/") ? baseUrl + methodUrl : baseUrl + "/" + methodUrl;
        }

        private static WWWResponse<string>? FindCorrectResponse([CanBeNull] WWWResponse<string>? mainResponse, Dictionary<ICancellableAsyncResult, WWWResponse<string>?>.ValueCollection mirrorResponses)
        {
            if (mainResponse != null)
            {
                if (mainResponse.Value.StatusCode == 200 ||
                    mainResponse.Value.StatusCode == 400 ||
                    mainResponse.Value.StatusCode == 401 ||
                    mainResponse.Value.StatusCode == 404)
                {
                    return mainResponse;
                }
            }

            foreach (var r in mirrorResponses)
            {
                if (r != null)
                {
                    if (r.Value.StatusCode == 200)
                    {
                        return r.Value;
                    }
                }
            }

            return null;
        }
    }
}
