using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// URL to app diff torrent.
	/// </summary>
	public struct AppDiffTorrentUrl
	{
		[JsonProperty("url")]
		public string Url;
	}
}