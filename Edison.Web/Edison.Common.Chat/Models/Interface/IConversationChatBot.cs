using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Common.Chat.Models.Interface
{
    public interface IConversationChatBot: IDisposable
    {
        IConnectorClient Create(Uri uri,MicrosoftAppCredentials appCreds);

        Task<string> SendReply(IConnectorClient client, Activity activity);

        Task<string> SendMessageToConversations(IConnectorClient client, Activity activity);
    }
}
