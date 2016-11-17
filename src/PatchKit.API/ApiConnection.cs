using System;
using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PatchKit.Async;

namespace PatchKit.Api
{
    /// <summary>
    /// PatchKit Api provider.
    /// </summary>
	public sealed partial class ApiConnection
    {
        private readonly IApiHttpDownloader _httpDownloader;

		private readonly ApiConnectionSettings _connectionSettings;

		private readonly JsonSerializerSettings _jsonSerializerSettings;

		public ApiConnection(ApiConnectionSettings connectionSettings, [NotNull] IApiHttpDownloader httpDownloader)
        {
            if (connectionSettings.Urls == null)
            {
                throw new ArgumentNullException("connectionSettings", "Empty url list in connection settings.");
            }
            if (connectionSettings.Timeout <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionSettings", "Invaild timeout (must be between 1 and Infinite).");
            }
            if (httpDownloader == null)
            {
                throw new ArgumentNullException("httpDownloader");
            }

            _connectionSettings = connectionSettings;
            _httpDownloader = httpDownloader;
			_jsonSerializerSettings = JsonConvert.DefaultSettings.Invoke();
			_jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			_jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
        }

        public ApiConnection(ApiConnectionSettings connectionSettings) : this(connectionSettings, new ApiHttpDownloader())
        {
        }

        public ApiConnection() : this(new ApiConnectionSettings(500, "http://api.patchkit.net"))
        {
        }

        private ICancellableAsyncResult BeginApiRequest<T>(string resource, CancellableAsyncCallback callback, object state)
		{
			var result = new AsyncResult<T> (cancellationToken => ApiRequest<T>(resource, cancellationToken), callback, state);

			return result;
		}

        private T EndApiRequest<T>(IAsyncResult asyncResult)
        {
            var result = asyncResult as AsyncResult<T>;

            if (result == null)
            {
                throw new ArgumentException("asyncResult");
            }

            return result.FetchResultsFromAsyncOperation();
        }

		private T ApiRequest<T>(string resource, AsyncCancellationToken cancellationToken)
		{
            // We want to save at least last request exception
            // TODO: Use some kind of AggregateException for it (sadly it's not available for .NET 3.5)
            Exception lastRequestException = null;

            foreach (var url in _connectionSettings.Urls)
		    {
                ApiHttpResult? result = null;

                // Join url with resource path and make sure that they are joined by only one '/' char
                string resourceUrl = url.TrimEnd('/') + "/" + resource.TrimStart('/');

                // Start download of string
		        var asyncResult = _httpDownloader.BeginDownloadString(resourceUrl, _connectionSettings.Timeout);

		        using (cancellationToken.Register(() => asyncResult.Cancel()))
		        {
		            // Wait for completion of request
		            asyncResult.AsyncWaitHandle.WaitOne(_connectionSettings.Timeout);

		            // Check if request has been completed and not cancelled
		            if (asyncResult.IsCompleted && !asyncResult.IsCancelled)
		            {
		                // Try to get result
		                try
		                {
		                    result = _httpDownloader.EndDownloadString(asyncResult);
		                }
		                catch (Exception exception)
		                {
		                    // Save the exception
		                    lastRequestException = exception;
		                }
		            }
		        }

		        if (result != null)
		        {
                    // Validate status code
		            if (result.Value.StatusCode != 200)
		            {
		                throw new ApiException("Invaild API response.", result.Value.StatusCode);
		            }

                    // Deserialize response content.
					return JsonConvert.DeserializeObject<T>(result.Value.Value, _jsonSerializerSettings);
                }
		    }

		    throw new WebException("Unable to download API resource.", lastRequestException);
		}
    }
}
