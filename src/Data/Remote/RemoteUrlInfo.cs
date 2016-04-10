using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    internal struct RemoteUrlInfo
    {
        [JsonProperty("url")]
        public string Url;
    }
}
