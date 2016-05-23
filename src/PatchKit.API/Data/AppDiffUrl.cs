using Newtonsoft.Json;

namespace PatchKit.API.Data
{
    /// <summary>
    /// URL to app diff.
    /// </summary>
    public struct AppDiffUrl
    {
        [JsonProperty("url")]
        public string Url;
    }
}