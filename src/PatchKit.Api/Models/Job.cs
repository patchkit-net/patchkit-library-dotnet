using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct Job
    {
        /// <summary>
        /// Job GUID to be used with Jobs API.
        /// </summary>
        [JsonProperty("job_guid")]
        public string JobGuid;
        
    }
}
