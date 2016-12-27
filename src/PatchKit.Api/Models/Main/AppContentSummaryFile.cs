using Newtonsoft.Json;

namespace PatchKit.Api.Models.Main
{
    public struct AppContentSummaryFile
    {
        /// <summary>
        /// File path.
        /// </summary>
        [JsonProperty("path")]
        public string Path;
        
        /// <summary>
        /// File hash.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash;
        
    }
}
