using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    /// <summary>
    /// Information about file included in content.
    /// </summary>
    internal struct RemoteContentFile
    {
        /// <summary>
        /// File name.
        /// </summary>
        [JsonProperty("path")]
        public string Path;

        /// <summary>
        /// Hash xxhash64.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash;
    }
}
