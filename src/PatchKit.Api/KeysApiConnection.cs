namespace PatchKit.Api
{
    /// <summary>
    /// PatchKit Keys Api Connection.
    /// </summary>
    public sealed partial class KeysApiConnection : ApiConnection
    {
        /// <summary>
        /// Returns default settings.
        /// </summary>
        public static ApiConnectionSettings GetDefaultSettings()
        {
            return new ApiConnectionSettings
            {
                MainServer = new ApiConnectionServer
                {
                    Host = "keys.patchkit.net",
                    Port = 43240,
                    UseHttps = false,
                    Timeout = 5000
                },
                CacheServers = null
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeysApiConnection"/> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings.</param>
        public KeysApiConnection(ApiConnectionSettings connectionSettings) : base(connectionSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeysApiConnection"/> class.
        /// </summary>
        public KeysApiConnection() : this(GetDefaultSettings())
        {
        }
    }
}