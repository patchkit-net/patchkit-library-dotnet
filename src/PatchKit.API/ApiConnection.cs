using System;
using System.Net;
using System.Threading;

namespace PatchKit.Api
{
    /// <summary>
    /// PatchKit Api Connection.
    /// </summary>
    public sealed class ApiConnection
    {
        private readonly ApiConnectionSettings _connectionSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiConnection"/> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings.</param>
        /// <exception cref="System.ArgumentNullException">
        /// connectionSettings - Url to main server is null.
        /// or
        /// connectionSettings - Url to main server is empty.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// connectionSettings - Invalid minimum timeout (must be between 1 and Infinite).
        /// or
        /// connectionSettings - Invalid maximum timeout (must be between minimum timeout and Infinite).
        /// </exception>
        public ApiConnection(ApiConnectionSettings connectionSettings)
        {
            if (connectionSettings.MainServer == null)
            {
                throw new ArgumentNullException("connectionSettings", "Url to main server is null.");
            }
            if (string.IsNullOrEmpty(connectionSettings.MainServer))
            {
                throw new ArgumentNullException("connectionSettings", "Url to main server is empty.");
            }
            if (connectionSettings.MinimumTimeout <= 0)
            {
                throw new ArgumentOutOfRangeException("connectionSettings",
                    "Invalid minimum timeout (must be between 1 and Infinite).");
            }
            if (connectionSettings.MaximumTimeout <= 0 ||
                connectionSettings.MaximumTimeout < connectionSettings.MinimumTimeout)
            {
                throw new ArgumentOutOfRangeException("connectionSettings",
                    "Invalid maximum timeout (must be between minimum timeout and Infinite).");
            }

            _connectionSettings = connectionSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiConnection"/> class.
        /// </summary>
        public ApiConnection() : this(new ApiConnectionSettings())
        {
        }

        private IApiResponse GetFromServer(string host, string path, int timeout, string query)
        {
            Uri uri = new UriBuilder
            {
                Host = host,
                Path = path,
                Query = query
            }.Uri;

            var httpRequest = WebRequest.Create(uri) as HttpWebRequest;

            if (httpRequest == null)
            {
                throw new FormatException(string.Format("Invaild API uri - {0}", uri));
            }

            httpRequest.Timeout = timeout;

            var response = (HttpWebResponse) httpRequest.GetResponse();

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new WebException(response.StatusDescription);
            }

            return new ApiResponse(response);
        }

        private bool TryGetFromServer(string host, string path, string query, int timeout, out IApiResponse apiResponse)
        {
            apiResponse = null;

            try
            {
                apiResponse = GetFromServer(host, path, timeout, query);
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool IsGetResponseValid(IApiResponse apiResponse)
        {
            if ((int) apiResponse.HttpWebResponse.StatusCode >= 400)
            {
                return false;
            }

            return true;
        }

        private bool TryGetFromCacheServer(string host, string path, string query, int timeout,
            out IApiResponse apiResponse)
        {
            if (TryGetFromServer(host, path, query, timeout, out apiResponse))
            {
                if (IsGetResponseValid(apiResponse))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryGetFromMainServer(string path, string query, int timeout, out IApiResponse apiResponse)
        {
            if (TryGetFromServer(_connectionSettings.MainServer, path, query, timeout, out apiResponse))
            {
                if (!IsGetResponseValid(apiResponse))
                {
                    throw new ApiException(apiResponse.HttpWebResponse.StatusDescription,
                        (int) apiResponse.HttpWebResponse.StatusCode);
                }

                return true;
            }

            return false;
        }

        private bool TryGet(string path, string query, int timeout, out IApiResponse apiResponse)
        {
            if (TryGetFromMainServer(path, query, timeout, out apiResponse))
            {
                return true;
            }

            if (_connectionSettings.CacheServers != null)
            {
                foreach (var cacheServer in _connectionSettings.CacheServers)
                {
                    if (TryGetFromCacheServer(cacheServer, path, query, timeout, out apiResponse))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves specified resource from API.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="query">The query of the resource.</param>
        /// <returns>Resource result.</returns>
        /// <exception cref="System.TimeoutException">API request has timed out.</exception>
        public IApiResponse Get(string path, string query)
        {
            IApiResponse apiResponse;

            if (!TryGet(path, query, _connectionSettings.MinimumTimeout, out apiResponse))
            {
                if (!TryGet(path, query, _connectionSettings.MaximumTimeout, out apiResponse))
                {
                    throw new TimeoutException("API request has timed out.");
                }
            }

            return apiResponse;
        }

        /// <summary>
        /// Retrieves specified resource from API.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="query">The query of the resource.</param>
        /// <param name="onSuccess">Callback when request was successful.</param>
        /// <param name="onFailed">Callback when request failed.</param>
        /// <returns>Resource result.</returns>
        public void GetAsync(string path, string query, Action<IApiResponse> onSuccess, Action<Exception> onFailed)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    var response = Get(path, query);
                    onSuccess(response);
                }
                catch (Exception exception)
                {
                    onFailed(exception);
                }
            });
        }
    }
}