using Newtonsoft.Json;

namespace PatchKit.Api.Models.Main
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
        /// Guid of content meta file if available.
        /// </summary>
        [JsonProperty("content_meta_guid")]
        public string ContentMetaGuid;
        
        /// <summary>
        /// Guid of diff file or null if there's no diff.
        /// </summary>
        [JsonProperty("diff_guid")]
        public string DiffGuid;
        
        /// <summary>
        /// Guid of diff meta file if available.
        /// </summary>
        [JsonProperty("diff_meta_guid")]
        public string DiffMetaGuid;
        
        /// <summary>
        /// Set to true if this version is a draft version.
        /// </summary>
        [JsonProperty("draft")]
        public bool Draft;
        
        /// <summary>
        /// Main executable relative path without slash at the beginning.
        /// </summary>
        [JsonProperty("main_executable")]
        public string MainExecutable;
        
        /// <summary>
        /// Main executable arguments
        /// </summary>
        [JsonProperty("main_executable_args")]
        public string[] MainExecutableArgs;
        
        /// <summary>
        /// Relative list of paths that should be ignored for local data consistency.
        /// </summary>
        [JsonProperty("ignored_files")]
        public string[] IgnoredFiles;
        
    }
}
