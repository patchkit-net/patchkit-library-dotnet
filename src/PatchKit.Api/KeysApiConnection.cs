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
                MainServer = "keys.patchkit.net",
                CacheServers = null,
                Timeout = 5000
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