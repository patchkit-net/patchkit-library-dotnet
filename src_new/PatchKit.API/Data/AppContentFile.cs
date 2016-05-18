using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// Information about file included in app content package.
	/// </summary>
	public struct AppContentFile
	{
		/// <summary>
		/// File name.
		/// </summary>
		[JsonProperty("path")]
		public string Path;

		/// <summary>
		/// Hash xxhash64.
		/// </summary>
		[JsonProperty("hash")]
		public string Hash;
	}
}