using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct AppDiffSummary
    {
        /// <summary>
        /// Diff size.
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
        /// List of added files.
        /// </summary>
        [JsonProperty("added_files")]
        public string[] AddedFiles;
        
        /// <summary>
        /// List of modified files.
        /// </summary>
        [JsonProperty("modified_files")]
        public string[] ModifiedFiles;
        
        /// <summary>
        /// List of removed files.
        /// </summary>
        [JsonProperty("removed_files")]
        public string[] RemovedFiles;
        
        [JsonProperty("hash_code")]
        public string HashCode;
        
        [JsonProperty("chunks")]
        public Chunks Chunks;
        
    }
}
