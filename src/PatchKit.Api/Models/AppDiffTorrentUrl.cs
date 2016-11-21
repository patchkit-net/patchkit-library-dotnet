using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct AppDiffTorrentUrl
    {
        /// <summary>
        /// Url to diff torrent file.
        /// </summary>
        [JsonProperty("url")]
        public string Url;
        
    }
}
