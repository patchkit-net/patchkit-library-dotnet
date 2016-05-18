using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App current version information.
	/// </summary>
    public struct AppLatestVersionID
    {
        [JsonProperty("id")]
        public int ID;
    }
}
