using Microsoft.Bot.Connector;

namespace Edison.Common.Chat.Models
{
    public class MessageRouterResult
    {
        public MessageRouterResultType Type{ get;set;}

        public Activity Activity { get; set; }
        
        public ConversationResourceResponse ConversationResourceResponse { get; set; }
       
        public Party ConversationAgentParty { get; set; }

        public Party ConversationClientParty { get; set; }

        public string ErrorMessage { get; set; }

        public MessageRouterResult()
        {
            Type = MessageRouterResultType.NoActionTaken;
            ErrorMessage = string.Empty;
        }

        public override string ToString()
        {
            return $"[{Type}; {ConversationResourceResponse}; {ConversationAgentParty}; {ConversationClientParty}; {ErrorMessage}]";
        }
    }
}
