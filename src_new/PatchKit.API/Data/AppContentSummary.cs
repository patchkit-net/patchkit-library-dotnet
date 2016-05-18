using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App content package summary.
	/// </summary>
	public struct AppContentSummary
	{
		[JsonProperty("size")]
		public long Size;

		[JsonProperty("encryption_method")]
		public string EncryptionMethod;

		[JsonProperty("compression_method")]
		public string CompressionMethod;

		[JsonProperty("files")]
		public AppContentFile[] Files;
	}
}