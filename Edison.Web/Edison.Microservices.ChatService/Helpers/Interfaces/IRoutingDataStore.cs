using Edison.ChatService.Models;
using Edison.Core.Common.Models;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;

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
