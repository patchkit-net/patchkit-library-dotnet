using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App latest version id.
	/// </summary>
    public struct AppLatestVersionId
    {
        [JsonProperty("id")]
        public int Id;
    }
}
