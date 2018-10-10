using Edison.Common.Chat.Models.Interface;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Common.Chat.Repositories
{
    public class ConversationChatBot : IConversationChatBot
    {
        private IConnectorClient connectorClient;

        public IConnectorClient Create(Uri uri, MicrosoftAppCredentials appCreds)
        {
            connectorClient= new ConnectorClient(uri, appCreds);
            return connectorClient;
        }

        public async Task<string> SendReply(IConnectorClient client, Activity activity)
        {
            var resourceResponse= await client.Conversations.ReplyToActivityAsync(activity);
            return activity.Text;
        }

        public async Task<string> SendMessageToConversations(IConnectorClient client, Activity activity)
        {
            var resourceResponse =
                    await client.Conversations.SendToConversationAsync(
                        activity);
            return activity.Text;
        }

        public void Dispose()
        {
            if (connectorClient != null)
            {
                connectorClient.Dispose();
            }
        }
    }
}
