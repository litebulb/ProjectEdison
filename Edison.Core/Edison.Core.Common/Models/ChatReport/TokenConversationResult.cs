using Newtonsoft.Json;

namespace Edison.Core.Common.Models
{
    public class TokenConversationResult
    {
        public string ConversationId { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int? ExpiresIn { get; set; }
        [JsonProperty(PropertyName = "streamUrl")]
        public string StreamUrl { get; set; }
        [JsonProperty(PropertyName = "referenceGrammarId")]
        public string ReferenceGrammarId { get; set; }
        [JsonProperty(PropertyName = "eTag")]
        public string ETag { get; set; }
    }
}
