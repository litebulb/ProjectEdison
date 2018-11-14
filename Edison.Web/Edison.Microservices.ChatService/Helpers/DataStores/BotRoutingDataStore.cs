using AutoMapper;
using Edison.ChatService.Helpers.Interfaces;
using Edison.ChatService.Models;
using Edison.Common.DAO;
using Edison.Common.Interfaces;
using Edison.Core.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.ChatService.Helpers
{
    /// <summary>
    /// Routing data store that stores the data in Azure Table Storage.
    /// See the IRoutingDataStore interface for the general documentation of properties and methods.
    /// </summary>
    [Serializable]
    public class BotRoutingDataStore : IRoutingDataStore
    {
        private ICosmosDBRepository<ChatUserSessionDAO> _repoChatUserSessions;

        public BotRoutingDataStore(ICosmosDBRepository<ChatUserSessionDAO> repoChatUserSessions)
        {
            _repoChatUserSessions = repoChatUserSessions;
        }

        public async Task<IEnumerable<ConversationReference>> GetConversationsFromUser(string userId)
        {
            var result = await _repoChatUserSessions.GetItemsAsync(p => p.Id == userId);
            return Mapper.Map<IEnumerable<ConversationReference>>(result);
        }

        public async Task<bool> DeleteUserConversation(string userId)
        {
            return await _repoChatUserSessions.DeleteItemAsync(userId);
        }

        public async Task<IEnumerable<ConversationReference>> GetConversations(ChatUserRole chatUserRole)
        {
            var result = await _repoChatUserSessions.GetItemsAsync(p => p.Role == chatUserRole.ToString());
            return Mapper.Map<IEnumerable<ConversationReference>>(result);
        }

        public async Task<IEnumerable<ConversationReference>> GetConversations()
        {
            var result = await _repoChatUserSessions.GetItemsAsync();
            return Mapper.Map<IEnumerable<ConversationReference>>(result);
        }

        public async Task<IEnumerable<ChatUserReadStatusModel>> GetUsersReadStatusPerUser(string userId)
        {
            var result = await _repoChatUserSessions.GetItemAsync(userId);
            return Mapper.Map<IEnumerable<ChatUserReadStatusModel>>(result?.UsersReadStatus);
        }

        public async Task<bool> AddConversationReference(ConversationReference conversationReference)
        {
            ChatUserSessionDAO newDAO = Mapper.Map<ChatUserSessionDAO>(conversationReference);

            ChatUserSessionDAO oldDAO = await _repoChatUserSessions.GetItemAsync(newDAO.Id);
            if(oldDAO == null)
            {
                string id = await _repoChatUserSessions.CreateItemAsync(newDAO);
                return !string.IsNullOrEmpty(id);
            }
            else
            {
                oldDAO.BotId = newDAO.BotId;
                oldDAO.BotName = newDAO.BotName;
                oldDAO.ChannelId = newDAO.ChannelId;
                oldDAO.ConversationId = newDAO.ConversationId;
                oldDAO.ServiceUrl = newDAO.ServiceUrl;
                oldDAO.Name = newDAO.Name;
                oldDAO.Role = newDAO.Role;

                try
                {
                    return await _repoChatUserSessions.UpdateItemAsync(oldDAO);
                }
                catch (DocumentClientException e)
                {
                    //Update concurrency issue, retrying
                    if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                        return await AddConversationReference(conversationReference);
                    throw e;
                }
            }
        }

        public async Task<bool> UpdateUserReadMessageStatus(string userId, ChatUserReadStatusModel chatUserReadStatus)
        {
            ChatUserSessionDAO userSessionDAO = await _repoChatUserSessions.GetItemAsync(userId);
            if (userSessionDAO == null)
                return false;

            ChatUserReadStatusDAOObject chatUserReadDAO = null;

            if (userSessionDAO.UsersReadStatus != null)
                chatUserReadDAO = userSessionDAO.UsersReadStatus.Find(p => p.UserId == chatUserReadStatus.UserId);
            else
                userSessionDAO.UsersReadStatus = new List<ChatUserReadStatusDAOObject>();

            //Doesn't exist, we add it.
            if (chatUserReadDAO == null)
            {
                chatUserReadDAO = new ChatUserReadStatusDAOObject
                {
                    UserId = chatUserReadStatus.UserId
                };
                userSessionDAO.UsersReadStatus.Add(chatUserReadDAO);
            }

            chatUserReadDAO.Date = chatUserReadStatus.Date;

            try
            {
                return await _repoChatUserSessions.UpdateItemAsync(userSessionDAO);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await UpdateUserReadMessageStatus(userId, chatUserReadStatus);
                throw e;
            }
        }
    }
}
