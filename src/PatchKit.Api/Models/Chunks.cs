using Newtonsoft.Json;

namespace PatchKit.Api.Models
{
    public struct Chunks
    {
        [JsonProperty("size")]
        public int Size;
        
        [JsonProperty("hashes")]
        public string[] Hashes;
        
    }
}
