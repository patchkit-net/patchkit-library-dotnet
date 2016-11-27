using System;
using System.Net;
using Newtonsoft.Json;

namespace PatchKit.Api
{
    /// <summary>
    /// Base Api Connection.
    /// </summary>
    public abstract class ApiConnection
    {
        private readonly ApiConnectionSettings _connectionSettings;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

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
        /// connectionSettings - Timeout value is less than zero and is not <see cref="System.Threading.Timeout.Infinite" />.
        /// </exception>
        protected ApiConnection(ApiConnectionSettings connectionSettings)
        {
            if (connectionSettings.MainServer == null)
            {
                throw new ArgumentNullException("connectionSettings", "Url to main server is null.");
            }
            if (string.IsNullOrEmpty(connectionSettings.MainServer))
            {
                throw new ArgumentNullException("connectionSettings", "Url to main server is empty.");
            }
            if (connectionSettings.Timeout < 0 && connectionSettings.Timeout != System.Threading.Timeout.Infinite)
            {
                throw new ArgumentOutOfRangeException("connectionSettings",
                    "Timeout value is less than zero and is not System.Threading.Timeout.Infinite.");
            }

            _connectionSettings = connectionSettings;
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            _jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
        }

        /// <summary>
        /// Parses the response data to structure.
        /// </summary>
        protected T ParseResponse<T>(IApiResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Body, _jsonSerializerSettings);
        }

        private bool IsResponseValid(IApiResponse response)
        {
            if ((int)response.HttpWebResponse.StatusCode >= 400)
            {
                return false;
            }

            return true;
        }

        private void ThrowIfResponseNotValid(IApiResponse response)
        {
            if (!IsResponseValid(response))
            {
                throw new ApiResponseException((int)response.HttpWebResponse.StatusCode);
            }
        }

        private HttpWebRequest CreateHttpRequest(Uri uri)
        {
            var httpRequest = WebRequest.Create(uri.ToString()) as HttpWebRequest;

            if (httpRequest == null)
            {
                throw new FormatException(string.Format("Invaild API uri - {0}", uri));
            }

            return httpRequest;
        }

        private bool TryGetHttpResponse(HttpWebRequest httpRequest, out HttpWebResponse httpResponse)
        {
            httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
            {
                return false;
            }

            return true;
        }

        private bool TryGetResponse(string host, string path, string query, int timeout, out IApiResponse response)
        {
            Uri uri = new UriBuilder
            {
                Scheme = "http",
                Host = host,
                Path = path,
                Query = query
            }.Uri;

            var httpRequest = CreateHttpRequest(uri);

            httpRequest.Timeout = timeout;

            response = null;

            try
            {
                HttpWebResponse httpResponse;

                if (!TryGetHttpResponse(httpRequest, out httpResponse))
                {
                    return false;
                }

                response = new ApiResponse(httpResponse);

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

        private bool TryGetResponseFromCacheServer(string host, string path, string query, int timeout,
            out IApiResponse response)
        {
            if (TryGetResponse(host, path, query, timeout, out response))
            {
                if (IsResponseValid(response))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryGetResponseFromMainServer(string path, string query, int timeout, out IApiResponse response)
        {
            if (TryGetResponse(_connectionSettings.MainServer, path, query, timeout, out response))
            {
                ThrowIfResponseNotValid(response);

                return true;
            }

            return false;
        }

        private bool TryGetResponse(string path, string query, int timeout, out IApiResponse apiResponse)
        {
            if (TryGetResponseFromMainServer(path, query, timeout, out apiResponse))
            {
                return true;
            }

            if (_connectionSettings.CacheServers != null)
            {
                foreach (var cacheServer in _connectionSettings.CacheServers)
                {
                    if (TryGetResponseFromCacheServer(cacheServer, path, query, timeout, out apiResponse))
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
        /// <returns>Response with resource result.</returns>
        /// <exception cref="ApiConnectionException">Could not connect to API.</exception>
        public IApiResponse GetResponse(string path, string query)
        {
            int timeout = _connectionSettings.Timeout;

            IApiResponse response;

            if (!TryGetResponse(path, query, timeout, out response))
            {
                // Double timeout and try again.

                timeout *= 2;

                if (!TryGetResponse(path, query, timeout, out response))
                {
                    throw new ApiConnectionException();
                }
            }

            return response;
        }
    }
}