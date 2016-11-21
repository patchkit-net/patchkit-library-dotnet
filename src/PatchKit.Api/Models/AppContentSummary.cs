using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct AppContentSummary
    {
        /// <summary>
        /// Content size.
        /// </summary>
        [JsonProperty("size")]
        public long Size;
        
        /// <summary>
        /// Encryption method.
        /// </summary>
        [JsonProperty("encryption_method")]
        public string EncryptionMethod;
        
        /// <summary>
        /// Compression method.
        /// </summary>
        [JsonProperty("compression_method")]
        public string CompressionMethod;
        
        /// <summary>
        /// List of content files.
        /// </summary>
        [JsonProperty("files")]
        public AppContentSummaryFile[] Files;
        
        [JsonProperty("hash_code")]
        public string HashCode;
        
        [JsonProperty("chunks")]
        public Chunks Chunks;
        
    }
}
