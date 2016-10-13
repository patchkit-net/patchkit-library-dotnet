using System;
using JetBrains.Annotations;

namespace PatchKit.Api
{
    /// <summary>
    /// <see cref="ApiConnection" /> settings.
    /// </summary>
    [Serializable]
    public struct ApiConnectionSettings
    {
        /// <summary>
        /// Returns default settings.
        /// </summary>
        public static ApiConnectionSettings CreateDefault()
        {
            return new ApiConnectionSettings
            {
                MainServer = "http://api.patchkit.net",
                CacheServers =
                    new[]
                    {
                        "api-cache-node-1.patchkit.net:43230", "api-cache-node-2.patchkit.net:43230",
                        "api-cache-node-3.patchkit.net:43230"
                    },
                MinimumTimeout = 5000,
                MaximumTimeout = 10000
            };
        }

        /// <summary>
        /// Url to main API server.
        /// </summary>
        [NotNull] public string MainServer;

        /// <summary>
        /// Urls for cache API servers. Priority of servers is based on the array order.
        /// </summary>
        [CanBeNull] public string[] CacheServers;

        /// <summary>
        /// Minimum timeout of single request to one API server.
        /// </summary>
        public int MinimumTimeout;

        /// <summary>
        /// Maximum timeout of single request to one API server.
        /// </summary>
        public int MaximumTimeout;
    }
}