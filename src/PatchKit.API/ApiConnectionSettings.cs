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
        /// Initializes a new instance of the <see cref="ApiConnectionSettings"/> struct.
        /// </summary>
        /// <param name="mainServer">Url to main server.</param>
        /// <param name="cacheServers">Urls to cache servers.</param>
        /// <param name="minimumTimeout">The minimum timeout.</param>
        /// <param name="maximumTimeout">The maximum timeout.</param>
        public ApiConnectionSettings([NotNull] string mainServer = "http://api.patchkit.net", [CanBeNull] string[] cacheServers = null, int minimumTimeout = 5000,
            int maximumTimeout = 10000) : this()
        {
            MainServer = mainServer;
            CacheServers = cacheServers;
            MinimumTimeout = minimumTimeout;
            MaximumTimeout = maximumTimeout;
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