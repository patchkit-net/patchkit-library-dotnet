using Newtonsoft.Json;

namespace PatchKit.Api.Models.Main
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
