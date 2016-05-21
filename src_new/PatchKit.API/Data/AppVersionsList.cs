using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// List of all app versions.
	/// </summary>
	public struct AppVersionsList
	{
		[JsonProperty("versions")]
		public AppVersion[] Versions;
	}
}