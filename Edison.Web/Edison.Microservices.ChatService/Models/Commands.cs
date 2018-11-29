using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Chat commands
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Commands
    {
        Undefined,
        GetTranscript,
        SendMessage,
        ReadUserMessages,
        EndConversation,
        Error
    }
}
