using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App latest version id.
	/// </summary>
    public struct AppLatestVersionId
    {
        /// <summary>
        /// Latest version id. If application does not have any version published then this value equals <c>0</c>.
        /// </summary>
        [JsonProperty("id")]
        public int Id;
    }
}
