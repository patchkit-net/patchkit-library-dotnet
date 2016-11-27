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
        /// Url to main API server.
        /// </summary>
        [NotNull] public string MainServer;

        /// <summary>
        /// Urls for cache API servers. Priority of servers is based on the array order.
        /// </summary>
        [CanBeNull] public string[] CacheServers;

        /// <summary>
        /// Timeout for connection with one server.
        /// </summary>
        public int Timeout;

        /// <summary>
        /// Total timeout of request - <see cref="Timeout"/> multipled by amount of servers (including main server).
        /// </summary>
        public int TotalTimeout
        {
            get
            {
                if (CacheServers == null)
                {
                    return Timeout;
                }
                return Timeout*(1 + CacheServers.Length);
            }
        }
    }
}