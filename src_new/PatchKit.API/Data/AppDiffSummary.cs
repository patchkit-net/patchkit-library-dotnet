using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App diff package summary.
	/// </summary>
	public struct AppDiffSummary
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