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
        /// connectionSettings - <see cref="ApiConnectionServer.Host"/> of one of the servers is null.
        /// or
        /// connectionSettings - <see cref="ApiConnectionServer.Host"/> of one of the servers is empty.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// connectionSettings - <see cref="ApiConnectionServer.Timeout"/> of one of the servers is less than zero and is not <see cref="System.Threading.Timeout.Infinite" />.
        /// </exception>
        public ApiConnection(ApiConnectionSettings connectionSettings)
        {
            ThrowIfServerIsInvalid(connectionSettings.MainServer);
            if (connectionSettings.CacheServers != null)
            {
                foreach (var cacheServer in connectionSettings.CacheServers)
                {
                    ThrowIfServerIsInvalid(cacheServer);
                }
            }

            _connectionSettings = connectionSettings;
            _jsonSerializerSettings = new JsonSerializerSettings();
            _jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            _jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
        }

        // ReSharper disable once UnusedParameter.Local
        private void ThrowIfServerIsInvalid(ApiConnectionServer server)
        {
            if (server.Host == null)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("connectionSettings", "ApiConnectionServer.Host of one of the servers is null.");
            }
            if (string.IsNullOrEmpty(server.Host))
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("connectionSettings", "ApiConnectionServer.Host of one of the servers is empty");
            }
            if (server.Timeout < 0 && server.Timeout != System.Threading.Timeout.Infinite)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentOutOfRangeException("connectionSettings",
                    "ApiConnectionServer.Timeout of one of the servers is less than zero and is not System.Threading.Timeout.Infinite");
            }
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

        private bool TryGetResponse(ApiConnectionServer server, string path, string query, int timeoutMultipler, ServerType serverType,
            out IApiResponse response)
        {
            Uri uri = new UriBuilder
            {
                Scheme = server.UseHttps ? "https" : "http",
                Host = server.Host,
                Path = path,
                Query = query,
                Port = server.RealPort
            }.Uri;

            var httpRequest = CreateHttpRequest(uri);

            httpRequest.Timeout = server.Timeout * timeoutMultipler;

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
            catch (WebException)
            {
                return false;
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

        private bool TryGetResponseFromCacheServer(ApiConnectionServer server, string path, string query, int timeoutMultipler,
            out IApiResponse response)
        {
            return TryGetResponse(server, path, query, timeoutMultipler, ServerType.CacheServer, out response);
        }

        private bool TryGetResponseFromMainServer(string path, string query, int timeoutMultipler, out IApiResponse response)
        {
            return TryGetResponse(_connectionSettings.MainServer, path, query, timeoutMultipler, ServerType.MainServer,
                out response);
        }

        private bool TryGetResponse(string path, string query, int timeoutMultipler, out IApiResponse apiResponse)
        {
            if (TryGetResponseFromMainServer(path, query, timeoutMultipler, out apiResponse))
            {
                return true;
            }

            if (_connectionSettings.CacheServers != null)
            {
                foreach (var cacheServer in _connectionSettings.CacheServers)
                {
                    if (TryGetResponseFromCacheServer(cacheServer, path, query, timeoutMultipler, out apiResponse))
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
            IApiResponse response;

            if (!TryGetResponse(path, query, 1, out response))
            {
                // Double timeout and try again.

                if (!TryGetResponse(path, query, 2, out response))
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