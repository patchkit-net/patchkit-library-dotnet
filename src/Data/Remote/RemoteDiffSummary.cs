using Newtonsoft.Json;

namespace PatchKit.Data.Remote
{
    internal struct RemoteDiffSummary
    {
        [JsonProperty("size")]
        public long Size;

        [JsonProperty("compression_method")]
        public string CompressionMethod;

        [JsonProperty("encryption_method")]
        public string EncryptionMethod;

        [JsonProperty("added_files")]
        public string[] AddedFiles;

        [JsonProperty("modified_files")]
        public string[] ModifiedFiles;

        [JsonProperty("removed_files")]
        public string[] RemovedFiles;
    }
}
