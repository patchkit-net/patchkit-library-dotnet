using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    internal struct RemoteResponse<T>
    {
        [JsonProperty("status")]
        public int Status;

        [JsonProperty("status_message")]
        public string StatusMessage;

        [JsonProperty("data")]
        public T Data;
    }
}
