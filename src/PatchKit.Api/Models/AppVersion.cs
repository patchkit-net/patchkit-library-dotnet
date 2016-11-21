using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct AppVersion
    {
        /// <summary>
        /// Initial version id.
        /// </summary>
        [JsonProperty("id")]
        public int Id;
        
        /// <summary>
        /// Version label.
        /// </summary>
        [JsonProperty("label")]
        public string Label;
        
        /// <summary>
        /// Description of changes.
        /// </summary>
        [JsonProperty("changelog")]
        public string Changelog;
        
        /// <summary>
        /// Unix timestamp of publish date.
        /// </summary>
        [JsonProperty("publish_date")]
        public long PublishDate;
        
        /// <summary>
        /// Guid of content file.
        /// </summary>
        [JsonProperty("content_guid")]
        public string ContentGuid;
        
        /// <summary>
        /// Guid of diff file or null if there's no diff.
        /// </summary>
        [JsonProperty("diff_guid")]
        public string DiffGuid;
        
        /// <summary>
        /// Set to true if this version is a draft version.
        /// </summary>
        [JsonProperty("draft")]
        public bool Draft;
        
    }
}
