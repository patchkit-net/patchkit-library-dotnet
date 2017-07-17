using System;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace PatchKit.Api
{
    /// <summary>
    /// Base Api Connection.
    /// </summary>
    public class ApiConnection
    {
        private readonly ApiConnectionSettings _connectionSettings;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public IHttpWebRequestFactory HttpWebRequestFactory = new WrapHttpWebRequestFactory();

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

        private IHttpWebRequest CreateHttpRequest(Uri uri)
        {
            IHttpWebRequest httpWebRequest = HttpWebRequestFactory.Create(uri.ToString());
            return httpWebRequest;
        }

        private bool TryGetResponse(string host, string path, string query, int timeout, ServerType serverType,
            out IApiResponse response)
        {
            int port;
            if (_connectionSettings.Port == 0)
            {
                port = _connectionSettings.UseHttps ? 443 : 80;
            }
            else
            {
                port = _connectionSettings.Port;
            }
            
            Uri uri = new UriBuilder
            {
                Scheme = _connectionSettings.UseHttps ? "https" : "http",
                Host = host,
                Path = path,
                Query = query,
                Port = port
            }.Uri;

            var httpRequest = CreateHttpRequest(uri);

            httpRequest.Timeout = timeout;

            response = null;

            try
            {
                var httpResponse = httpRequest.GetResponse();
                if (IsResponseValid(httpResponse, serverType))
                {
                    response = new ApiResponse(httpResponse);
                    return true;
                }

                if (IsResponseUnexpectedError(httpResponse, serverType))
                {
                    throw new ApiConnectionException("Server '" + httpRequest.Address.Host + "' returned code " +
                                                     (int) httpResponse.StatusCode);
                }

                return false;
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

        private bool IsResponseValid(IHttpWebResponse httpResponse, ServerType serverType)
        {
            switch (serverType)
            {
                case ServerType.MainServer:
                    return IsStatusCodeOK(httpResponse.StatusCode);
                case ServerType.CacheServer:
                    return httpResponse.StatusCode == HttpStatusCode.OK;
                default:
                    throw new ArgumentOutOfRangeException("serverType", serverType, null);
            }
        }

        private bool IsResponseUnexpectedError(IHttpWebResponse httpResponse, ServerType serverType)
        {
            switch (serverType)
            {
                case ServerType.MainServer:
                    return !IsStatusCodeOK(httpResponse.StatusCode) &&
                           !IsStatusCodeServerError(httpResponse.StatusCode);
                case ServerType.CacheServer:
                    return false; // ignore any api cache error
                default:
                    throw new ArgumentOutOfRangeException("serverType", serverType, null);
            }
        }

        private bool IsStatusCodeOK(HttpStatusCode statusCode)
        {
            return IsWithin((int) statusCode, 200, 299);
        }

        private bool IsStatusCodeServerError(HttpStatusCode statusCode)
        {
            return IsWithin((int) statusCode, 500, 599);
        }

        private bool IsWithin(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        private bool TryGetResponseFromCacheServer(string host, string path, string query, int timeout,
            out IApiResponse response)
        {
            return TryGetResponse(host, path, query, timeout, ServerType.CacheServer, out response);
        }

        private bool TryGetResponseFromMainServer(string path, string query, int timeout, out IApiResponse response)
        {
            return TryGetResponse(_connectionSettings.MainServer, path, query, timeout, ServerType.MainServer,
                out response);
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
                    throw new ApiConnectionException("Unable to connect to API.");
                }
            }

            return response;
        }

        private enum ServerType
        {
            MainServer,
            CacheServer,
        }
    }
}