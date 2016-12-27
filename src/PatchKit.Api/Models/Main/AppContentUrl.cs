using Newtonsoft.Json;

namespace PatchKit.Api.Models.Main
{
    public struct AppContentUrl
    {
        /// <summary>
        /// Url to content file.
        /// </summary>
        [JsonProperty("url")]
        public string Url;
        
    }
}
