using Newtonsoft.Json;

namespace PatchKit.API.Data
{
    /// <summary>
    /// URL to app content.
    /// </summary>
    public struct AppContentUrl
    {
        [JsonProperty("url")]
        public string Url;
    }
}