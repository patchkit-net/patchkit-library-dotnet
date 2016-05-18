using Newtonsoft.Json;

namespace PatchKit.API.Data
{
	/// <summary>
	/// App version.
	/// </summary>
	public struct AppVersion
	{
		[JsonProperty("id")]
		public int ID;

		[JsonProperty("label")]
		public string Label;

		[JsonProperty("changelog")]
		public string Changelog;

		[JsonProperty("publish_time")]
		public int PublishTime;

        [JsonProperty("content_guid")]
        public string ContentGuid;

        [JsonProperty("diff_guid")]
        public string DiffGuid;
    }
}

