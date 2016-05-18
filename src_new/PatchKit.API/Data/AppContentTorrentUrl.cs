using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// URL to app content torrent.
	/// </summary>
	public struct AppContentTorrentUrl
	{
		[JsonProperty("url")]
		public string Url;
	}
}