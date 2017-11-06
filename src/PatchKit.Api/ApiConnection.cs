using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using PatchKit.Logging;

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

        public ILogger Logger = DummyLogger.Instance;
        
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
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
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

        private bool TryGetResponse(ApiConnectionServer server, string path, string query, int timeoutMultipler, List<Exception> exceptionsList, ServerType serverType,
            out IApiResponse response)
        {
            Logger.LogDebug($"Trying to get response from server ({serverType}): '{server.Host}:{server.RealPort}' (uses HTTPS: {server.UseHttps})...");
            
            response = null;
            
            try
            {
                var uri = new UriBuilder
                {
                    Scheme = server.UseHttps ? "https" : "http",
                    Host = server.Host,
                    Path = path,
                    Query = query,
                    Port = server.RealPort
                }.Uri;

                var httpRequest = CreateHttpRequest(uri);

                httpRequest.Timeout = server.Timeout * timeoutMultipler;
                Logger.LogTrace($"Setting request timeout to {httpRequest.Timeout}ms");
                
                var httpResponse = httpRequest.GetResponse();

                Logger.LogDebug("Received response. Checking whether it is valid...");
                Logger.LogTrace($"Response status code: {httpResponse.StatusCode}");
                
                if (IsResponseValid(httpResponse, serverType))
                {
                    Logger.LogDebug("Response is valid.");
                    response = new ApiResponse(httpResponse);
                    return true;
                }

                Logger.LogWarning("Response is not valid. Checking whether it is API error...");
                
                if (IsResponseUnexpectedError(httpResponse, serverType))
                {
                    Logger.LogError("API error. Unable to get valid response.");
                    throw new ApiResponseException((int) httpResponse.StatusCode);
                }

                Logger.LogDebug("Error is not related to API. Registering it in exception list. Probably it was caused by connection problems.");
                exceptionsList.Add(new ApiException("Server '" + server.Host + "' returned code " +
                                                    (int) httpResponse.StatusCode));
                return false;
            }
            catch (WebException webException)
            {
                Logger.LogWarning("Unable to get response from server.", webException);
                exceptionsList.Add(webException);
                return false;
            }
        }

        private bool IsResponseValid(IHttpWebResponse httpResponse, ServerType serverType)
        {
            switch (serverType)
            {
                case ServerType.MainServer:
                    return IsStatusCodeOk(httpResponse.StatusCode);
                case ServerType.CacheServer:
                    return httpResponse.StatusCode == HttpStatusCode.OK;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }
        }

        private bool IsResponseUnexpectedError(IHttpWebResponse httpResponse, ServerType serverType)
        {
            switch (serverType)
            {
                case ServerType.MainServer:
                    return !IsStatusCodeOk(httpResponse.StatusCode) &&
                           !IsStatusCodeServerError(httpResponse.StatusCode);
                case ServerType.CacheServer:
                    return false; // ignore any api cache error
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }
        }

        private bool IsStatusCodeOk(HttpStatusCode statusCode)
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

        private bool TryGetResponseFromCacheServer(ApiConnectionServer server, string path, string query, int timeoutMultipler, ApiConnectionException apiConnectionException,
            out IApiResponse response)
        {
            return TryGetResponse(server, path, query, timeoutMultipler, apiConnectionException.CacheServersExceptionsList, ServerType.CacheServer, out response);
        }

        private bool TryGetResponseFromMainServer(string path, string query, int timeoutMultipler, ApiConnectionException apiConnectionException, out IApiResponse response)
        {
            return TryGetResponse(_connectionSettings.MainServer, path, query, timeoutMultipler, apiConnectionException.MainServerExceptionsList, ServerType.MainServer,
                out response);
        }

        private bool TryGetResponse(string path, string query, int timeoutMultipler, ApiConnectionException apiConnectionException, out IApiResponse apiResponse)
        {
            if (TryGetResponseFromMainServer(path, query, timeoutMultipler, apiConnectionException, out apiResponse))
            {
                return true;
            }

            if (_connectionSettings.CacheServers != null)
            {
                foreach (var cacheServer in _connectionSettings.CacheServers)
                {
                    if (TryGetResponseFromCacheServer(cacheServer, path, query, timeoutMultipler, apiConnectionException, out apiResponse))
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
            Logger.LogDebug($"Getting response for path: '{path}' and query: '{query}'...");
            
            IApiResponse response;

            var apiConnectionException = new ApiConnectionException();
            
            if (!TryGetResponse(path, query, 1, apiConnectionException, out response))
            {
                // Double timeout and try again.

                Logger.LogWarning(
                    "Failed to get response with regular timeout. Trying again with double timeout...");
                
                if (!TryGetResponse(path, query, 2, apiConnectionException, out response))
                {
                    Logger.LogError("Failed to get response with double timeout.");
                    
                    throw apiConnectionException;
                }
            }

            Logger.LogDebug("Successfully got response.");
            Logger.LogTrace($"Response body: {response.Body}");
            
            return response;
        }

        private enum ServerType
        {
            MainServer,
            CacheServer,
        }
    }
}