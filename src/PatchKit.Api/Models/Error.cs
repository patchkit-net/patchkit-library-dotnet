using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct Error
    {
        /// <summary>
        /// Human-readable error message
        /// </summary>
        [JsonProperty("message")]
        public string Message;
        
    }
}
