using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App content package summary.
	/// </summary>
	public struct AppContentSummary
	{
        /// <summary>
        /// Content archive file size in bytes (can be used for download progress display).
        /// </summary>
		[JsonProperty("size")]
		public long Size;

        /// <summary>
        /// Used encryption method.
        /// * none - Data not encrypted.
        /// * zip - Zip archive with password.
        /// </summary>
        [JsonProperty("encryption_method")]
		public string EncryptionMethod;

        /// <summary>
        /// Compression method.
        /// * zip - Zip archive.
        /// * xv - LMZA2 using xv.
        /// </summary>
		[JsonProperty("compression_method")]
		public string CompressionMethod;

        /// <summary>
        /// List of files within archive.
        /// </summary>
		[JsonProperty("files")]
		public AppContentFile[] Files;
	}
}