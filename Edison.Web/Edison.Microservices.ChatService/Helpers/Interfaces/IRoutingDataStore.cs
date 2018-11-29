using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Edison.Core.Common.Models;
using Edison.ChatService.Models;

namespace Edison.ChatService.Helpers.Interfaces
{
    public interface IRoutingDataStore
    {
        Task<IEnumerable<ConversationReference>> GetConversationsFromUser(string userId);
        Task<IEnumerable<ConversationReference>> GetConversations(ChatUserRole chatUserRole);
        Task<IEnumerable<ConversationReference>> GetConversations();
        Task<bool> AddConversationReference(ConversationReference conversationReferenceToAdd);
        Task<IEnumerable<ChatUserReadStatusModel>> GetUsersReadStatusPerUser(string userId);
        Task<bool> UpdateUserReadMessageStatus(string userId, ChatUserReadStatusModel chatUserReadStatus);
        Task<bool> DeleteUserConversation(string userId);
    }
}
