using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App version.
	/// </summary>
	public struct AppVersion
	{
        /// <summary>
        /// Version Id.
        /// </summary>
		[JsonProperty("id")]
		public int Id;

        /// <summary>
        /// Version label.
        /// </summary>
		[JsonProperty("label")]
		public string Label;

        /// <summary>
        /// Version changelog.
        /// </summary>
		[JsonProperty("changelog")]
		public string Changelog;

        /// <summary>
        /// Unix timestamp of publish date and time. 
        /// </summary>
		[JsonProperty("publish_time")]
		public long PublishTime;

        /// <summary>
        /// Content file GUID.
        /// </summary>
        [JsonProperty("content_guid")]
        public string ContentGuid;

        /// <summary>
        /// Diff file GUID.
        /// </summary>
        [JsonProperty("diff_guid")]
        public string DiffGuid;
    }
}