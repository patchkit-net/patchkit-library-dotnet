using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct AppContentTorrentUrl
    {
        /// <summary>
        /// Url to content torrent file.
        /// </summary>
        [JsonProperty("url")]
        public string Url;
        
    }
}
