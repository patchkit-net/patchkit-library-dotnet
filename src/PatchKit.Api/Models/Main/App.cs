using Newtonsoft.Json;

namespace PatchKit.Api.Models.Main
{
    public struct App
    {
        /// <summary>
        /// Initial app id.
        /// </summary>
        [JsonProperty("id")]
        public int Id;
        
        /// <summary>
        /// Application name
        /// </summary>
        [JsonProperty("name")]
        public string Name;
        
        /// <summary>
        /// Application display name.
        /// </summary>
        [JsonProperty("display_name")]
        public string DisplayName;
        
        /// <summary>
        /// Application author.
        /// </summary>
        [JsonProperty("author")]
        public string Author;
        
        /// <summary>
        /// The secret of patcher to use.
        /// </summary>
        [JsonProperty("patcher_secret")]
        public string PatcherSecret;
        
        /// <summary>
        /// If set to true, application needs to contact keys server to get valid key_secret for content download.
        /// </summary>
        [JsonProperty("use_keys")]
        public bool UseKeys;
        
        [JsonProperty("publish_method")]
        public string PublishMethod;
        
    }
}
