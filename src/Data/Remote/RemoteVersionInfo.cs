using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    public struct RemoteVersionInfo
    {
        [JsonProperty("id")]
        public int ID;

        [JsonProperty("label")]
        public string Label;

        [JsonProperty("changelog")]
        public string Changelog;

        [JsonProperty("publish_time")]
        public int PublishTime;
    }
}
