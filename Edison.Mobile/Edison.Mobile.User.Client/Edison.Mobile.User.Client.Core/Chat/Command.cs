using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edison.Mobile.User.Client.Core.Chat
{
    /// <summary>
    /// The commands.
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

    /// <summary>
    /// Command representation.
    /// </summary>
    public class Command
    {
        public const string CommandKeyword = "command"; // Used if the channel does not support mentions
        public const string CommandParameterAll = "*";

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

        public Command()
        {
        }
    }
}
