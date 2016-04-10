using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    internal struct RemoteCurrentVersionInfo
    {
        [JsonProperty("published")]
        public int Published;
    }
}
