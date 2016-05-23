using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App diff package summary.
	/// </summary>
	public struct AppDiffSummary
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
        /// List of new files. In archive these files will have the original content.
        /// </summary>
		[JsonProperty("added_files")]
		public string[] AddedFiles;

        /// <summary>
        /// List of modified files. In archive these filles will be binary diffs.
        /// </summary>
		[JsonProperty("modified_files")]
		public string[] ModifiedFiles;

        /// <summary>
        /// List of removed files.
        /// </summary>
		[JsonProperty("removed_files")]
		public string[] RemovedFiles;
	}
}