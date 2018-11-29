using Newtonsoft.Json;

namespace Edison.Core.Common.Models
{
    public class ChatUserModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "role")]
        public ChatUserRole Role { get; set; }
    }
}
