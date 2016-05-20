using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var mirrorRequests = new Dictionary<ICancellableAsyncResult, WWWResponse<string>?>();

            ICancellableAsyncResult mainRequest = null;

			object mainResponseLock = new object ();

            WWWResponse<string>? mainResponse = null;

            Exception mainException = null;

            WWWResponse<string>? correctResponse = null;

            try
            {
                int requestsCount = 1;

                object requestsCountLock = new object();

                mainRequest = _www.BeginDownloadString(GetUrl(_settings.APIUrl, methodUrl),
                        ar =>
                        {
                            try
                            {
                                if (!ar.HasBeenCancelled)
                                {
									lock(mainResponseLock)
									{
	                                    mainResponse = _www.EndDownloadString(ar);
									}
                                }
                            }
                            catch (Exception exception)
                            {
                                mainException = exception;
                            }
                            finally
                            {
                                lock (requestsCountLock)
                                {
                                    requestsCount--;

                                    if (requestsCount <= 0)
                                    {
                                        Monitor.PulseAll(requestsCountLock);
                                    }
                                }
                            }
                        }, null);



                foreach (var mirrorUrl in _settings.MirrorAPIUrls)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    lock (requestsCountLock)
                    {
                        if (requestsCount > 0)
                        {
                            Monitor.Wait(requestsCountLock, 5000);
                        }
                    }

					cancellationToken.ThrowIfCancellationRequested();

                    lock (mirrorRequests)
                    {
						lock(mainResponseLock)
						{
                        	correctResponse = FindCorrectResponse(mainResponse, mirrorRequests.Values);
						}
                    }

                    if (correctResponse != null)
                    {
                        break;
                    }

                    lock (mirrorRequests)
                    {
                        lock (requestsCountLock)
                        {
                            requestsCount++;
                        }

                        mirrorRequests.Add(_www.BeginDownloadString(GetUrl(mirrorUrl, methodUrl), ar =>
                        {
							try
							{
	                            lock (mirrorRequests)
	                            {
	                                mirrorRequests[ar] = _www.EndDownloadString(ar);
	                            }
							}
							finally
							{
								lock (requestsCountLock)
								{
									requestsCount--;
									
									if (requestsCount <= 0)
									{
										Monitor.PulseAll(requestsCountLock);
									}
								}
							}
                        }, null), null);
                    }
                }

                lock (requestsCountLock)
                {
                    while (requestsCount > 0 && correctResponse == null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

						lock (mirrorRequests)
						{
							lock(mainResponseLock)
							{
								correctResponse = FindCorrectResponse(mainResponse, mirrorRequests.Values);
							}
						}

                        Monitor.Wait(requestsCountLock, 10);
                    }
                }

				lock (mirrorRequests)
				{
					lock(mainResponseLock)
					{
						correctResponse = FindCorrectResponse(mainResponse, mirrorRequests.Values);
					}
				}

				if(correctResponse == null)
				{
					if(mainException != null)
					{
						throw mainException;
					}

					if(mainResponse != null)
					{
						throw new PatchKitAPIException("Unexcepted API response." + mainResponse.Value.StatusCode, mainResponse.Value.StatusCode);
					}
				}
				else
				{
					if (correctResponse.Value.StatusCode == 200)
					{
						return JsonConvert.DeserializeObject<T>(correctResponse.Value.Value);
					}
					else
					{
						throw new PatchKitAPIException("Unexcepted API response." + mainResponse.Value.StatusCode, correctResponse.Value.StatusCode);
					}
				}

				// This exception should not be thrown. TODO: Test to check it. 
				throw new Exception("Unable to get response from servers.");
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
