using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct AppVersionId
    {
        /// <summary>
        /// Version id.
        /// </summary>
        [JsonProperty("id")]
        public int Id;
        
    }
}
