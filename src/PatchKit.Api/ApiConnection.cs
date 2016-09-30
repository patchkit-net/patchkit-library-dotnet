using System;
using System.Net;

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
        public ApiConnection() : this(ApiConnectionSettings.CreateDefault())
        {
        }

        bool IsResponseValid(IApiResponse apiResponse)
        {
            if ((int)apiResponse.HttpWebResponse.StatusCode >= 400)
            {
                return false;
            }

            return true;
        }

        void ThrowIfResponseNotValid(IApiResponse apiResponse)
        {
            if (!IsResponseValid(apiResponse))
            {
                throw new ApiResponseException((int)apiResponse.HttpWebResponse.StatusCode);
            }
        }

        private bool TryGetFromServer(string host, string path, string query, int timeout, out IApiResponse apiResponse)
        {
            Uri uri = new UriBuilder
            {
                Scheme = "http",
                Host = host,
                Path = path,
                Query = query
            }.Uri;

            var httpRequest = WebRequest.Create(uri.ToString()) as HttpWebRequest;

            if (httpRequest == null)
            {
                throw new FormatException(string.Format("Invaild API uri - {0}", uri));
            }

            httpRequest.Timeout = timeout;

            apiResponse = null;

            try
            {
                var response = (HttpWebResponse)httpRequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    return false;
                }

                apiResponse = new ApiResponse(response);

                return true;
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.Timeout)
                {
                    return false;
                }
                throw;
            }
        }

        private bool TryGetFromCacheServer(string host, string path, string query, int timeout,
            out IApiResponse apiResponse)
        {
            if (TryGetFromServer(host, path, query, timeout, out apiResponse))
            {
                if (IsResponseValid(apiResponse))
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
                ThrowIfResponseNotValid(apiResponse);

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
                    throw new ApiConnectionException();
                }
            }

            return apiResponse;
        }
    }
}