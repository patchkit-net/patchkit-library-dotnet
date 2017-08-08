namespace PatchKit.Api
{
    /// <summary>
    /// PatchKit Main Api Connection.
    /// </summary>
    public sealed partial class MainApiConnection : ApiConnection
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
                    Host = "api.patchkit.net",
                    UseHttps = false,
                    Port = 0,
                    Timeout = 5000
                },
                CacheServers =
                    new[]
                    {
                        new ApiConnectionServer
                        {
                            Host = "api-cache-1.patchkit.net",
                            Port = 43230,
                            UseHttps = false,
                            Timeout = 5000
                        },
                        new ApiConnectionServer
                        {
                            Host = "api-cache-2.patchkit.net",
                            Port = 43230,
                            UseHttps = false,
                            Timeout = 5000
                        },
                        new ApiConnectionServer
                        {
                            Host = "api-cache-3.patchkit.net",
                            Port = 43230,
                            UseHttps = false,
                            Timeout = 5000
                        }
                    }
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApiConnection"/> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings.</param>
        public MainApiConnection(ApiConnectionSettings connectionSettings) : base(connectionSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainApiConnection"/> class.
        /// </summary>
        public MainApiConnection() : this(GetDefaultSettings())
        {
        }
    }
}