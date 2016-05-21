using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// Url to app content torrent.
	/// </summary>
	public struct AppContentTorrentUrl
	{
		[JsonProperty("url")]
		public string Url;
	}
}