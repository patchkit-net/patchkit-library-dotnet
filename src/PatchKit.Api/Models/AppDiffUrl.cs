using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct AppDiffUrl
    {
        /// <summary>
        /// Url to diff file.
        /// </summary>
        [JsonProperty("url")]
        public string Url;
        
    }
}
