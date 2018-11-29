using Newtonsoft.Json;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Object Command
    /// </summary>
    public class Command
    {
        [JsonProperty(PropertyName = "baseCommand")]
        public Commands BaseCommand
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "data")]
        public object Data
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "botName")]
        public string BotName
        {
            get;
            set;
        }
    }
}
