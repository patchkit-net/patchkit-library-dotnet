using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct LicenseKey
    {
        [JsonProperty("key")]
        public string Key;
        
        [JsonProperty("app_secret")]
        public string AppSecret;
        
        [JsonProperty("key_secret")]
        public string KeySecret;
        
        [JsonProperty("collection_id")]
        public int CollectionId;
        
    }
}
