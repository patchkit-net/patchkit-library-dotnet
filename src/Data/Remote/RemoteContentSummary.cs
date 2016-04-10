using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    internal struct RemoteContentSummary
    {
        [JsonProperty("size")]
        public long Size;

        [JsonProperty("encryption_method")]
        public string EncryptionMethod;

        [JsonProperty("compression_method")]
        public string CompressionMethod;

        [JsonProperty("files")]
        public RemoteContentFile[] Files;
    }
}
