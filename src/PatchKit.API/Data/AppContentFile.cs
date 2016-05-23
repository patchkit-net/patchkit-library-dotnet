using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// Information about file included in app content package.
	/// </summary>
	public struct AppContentFile
	{
        /// <summary>
        /// Relative file path.
        /// </summary>
        [JsonProperty("path")]
		public string Path;

        /// <summary>
        /// File hash.
        /// </summary>
        [JsonProperty("hash")]
		public string Hash;
	}
}