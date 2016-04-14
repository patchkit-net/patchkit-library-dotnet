using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    public struct RemoteAllVersionsInfo
    {
        [JsonProperty("versions")]
        public RemoteVersionInfo[] Versions;
    }
}
