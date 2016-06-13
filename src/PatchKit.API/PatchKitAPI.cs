using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PatchKit.API.Async;
using PatchKit.API.Web;

namespace PatchKit.API
{
    /// <summary>
    /// PatchKit API provider.
    /// </summary>
    public partial class PatchKitAPI
    {
        private readonly IStringDownloader _stringDownloader;

        private readonly PatchKitAPISettings _settings;

        public PatchKitAPI([NotNull] PatchKitAPISettings settings, [NotNull] IStringDownloader stringDownloader)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (stringDownloader == null)
            {
                throw new ArgumentNullException("stringDownloader");
            }

            _settings = settings;
            _stringDownloader = stringDownloader;
        }

        public PatchKitAPI([NotNull] PatchKitAPISettings settings) : this(settings, new StringDownloader())
        {
        }

        public PatchKitAPI() : this(new PatchKitAPISettings())
        {
        }

        private ICancellableAsyncResult BeginAPIRequest<T>(string url, CancellableAsyncCallback callback, object state)
		{
			var result = new AsyncResult<T> (cancellationToken => DownloadAndVerifyServerResponse<T>(url, cancellationToken), callback, state);

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
            var mirrorRequests = new Dictionary<ICancellableAsyncResult, StringDownloadResult?>();

            ICancellableAsyncResult mainRequest = null;

            StringDownloadResult?[] mainDownloadResult = {null};

            Exception[] mainException = {null};

            StringDownloadResult? correctDownloadResult = null;

            object requestsLock = new object();

            try
            {
                // Register cancellation callback which pulses the request lock in order to unfreeze main operation thread.
                using (cancellationToken.Register(() => Monitor.PulseAll(requestsLock)))
                {
                    // Begin with main request.
                    mainRequest = _stringDownloader.BeginDownloadString(GetUrl(_settings.Url, methodUrl),
                        ar =>
                        {
                            try
                            {
                                if (!ar.IsCancelled)
                                {
                                    lock (requestsLock)
                                    {
                                        // Save the main response.
                                        mainDownloadResult[0] = _stringDownloader.EndDownloadString(ar);
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
                    if (_settings.MirrorUrls != null)
                    {
                        foreach (var mirrorUrl in _settings.MirrorUrls)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            lock (requestsLock)
                            {
                                // Wait 5 seconds, until a request is finished or until operation is cancelled.
                                if (GetNumberOfUncompletedRequests(mainRequest, mirrorRequests.Keys) > 0)
                                {
                                    Monitor.Wait(requestsLock, (int) _settings.DelayBetweenMirrorRequests);
                                }
                            }

                            cancellationToken.ThrowIfCancellationRequested();

                            lock (requestsLock)
                            {
                                correctDownloadResult = FindCorrectResult(mainDownloadResult[0], mirrorRequests.Values);
                            }

                            // If correct response is found, leave the foreach.
                            if (correctDownloadResult != null)
                            {
                                break;
                            }

                            lock (requestsLock)
                            {
                                // Make new mirror request.
                                mirrorRequests.Add(_stringDownloader.BeginDownloadString(GetUrl(mirrorUrl, methodUrl), ar =>
                                {
                                    try
                                    {
                                        lock (requestsLock)
                                        {
                                            // Save mirror response.
                                            mirrorRequests[ar] = _stringDownloader.EndDownloadString(ar);
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
                    }

                    lock (requestsLock)
                    {
                        // Check whether there are uncompleted requests and correct response is still not found.
                        while (GetNumberOfUncompletedRequests(mainRequest, mirrorRequests.Keys) > 0 &&
                               correctDownloadResult == null)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            lock (requestsLock)
                            {
                                correctDownloadResult = FindCorrectResult(mainDownloadResult[0], mirrorRequests.Values);
                            }

                            // Wait until a request is finished or until operation is cancelled.
                            Monitor.Wait(requestsLock);
                        }
                    }

                    lock (requestsLock)
                    {
                        correctDownloadResult = FindCorrectResult(mainDownloadResult[0], mirrorRequests.Values);
                    }

                    // If correct response hasn't arrived from any source.
                    if (correctDownloadResult == null)
                    {
                        // Rethrow exception from main request (if exists).
                        if (mainException[0] != null)
                        {
                            throw mainException[0];
                        }

                        // Throw API exception containing status code from the response (if exists).
                        if (mainDownloadResult[0] != null)
                        {
                            throw new PatchKitAPIException("Unexcepted API response." + mainDownloadResult[0].Value.StatusCode, mainDownloadResult[0].Value.StatusCode);
                        }
                    }
                    else
                    {
                        // Check whether response status code is correct.
                        if (correctDownloadResult.Value.StatusCode == 200)
                        {
                            // Deserialize response content.
                            return JsonConvert.DeserializeObject<T>(correctDownloadResult.Value.Value);
                        }
                        else
                        {
                            // Throw API exception containing status code from the response.
                            throw new PatchKitAPIException("Unexcepted API response." + correctDownloadResult.Value.StatusCode, correctDownloadResult.Value.StatusCode);
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
            Dictionary<ICancellableAsyncResult, StringDownloadResult?>.KeyCollection mirrorRequests)
        {
            return mirrorRequests.Count(r => !r.IsCompleted) + (mainRequest.IsCompleted ? 0 : 1);
        }

        private static string GetUrl(string baseUrl, string methodUrl)
        {
            return baseUrl.EndsWith("/") ? baseUrl + methodUrl : baseUrl + "/" + methodUrl;
        }

        private static StringDownloadResult? FindCorrectResult([CanBeNull] StringDownloadResult? mainDownloadResult, Dictionary<ICancellableAsyncResult, StringDownloadResult?>.ValueCollection mirrorDownloadResults)
        {
            if (mainDownloadResult != null)
            {
                if (mainDownloadResult.Value.StatusCode == 200 ||
                    mainDownloadResult.Value.StatusCode == 400 ||
                    mainDownloadResult.Value.StatusCode == 401 ||
                    mainDownloadResult.Value.StatusCode == 404)
                {
                    return mainDownloadResult;
                }
            }

            foreach (var r in mirrorDownloadResults)
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

#if NET45
        private static System.Threading.Tasks.Task<T> ToAsync<T>(Func<CancellableAsyncCallback, ICancellableAsyncResult> beginMethod,
            Func<IAsyncResult, T> endMethod, CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.Factory.FromAsync((callback, o) =>
            {
                var asyncResult = beginMethod(ar => callback(ar));

                cancellationToken.Register(() => asyncResult.Cancel());

                return asyncResult;
            }, endMethod, null);
        }
#endif
    }
}
