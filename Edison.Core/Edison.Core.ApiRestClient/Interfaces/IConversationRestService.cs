using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IConversationRestService
    {
        Task<ReportModel> GetConversationById(Guid conversationId);
        Task<ReportModel> GetActiveConversationFromUser(string userId);
        Task<IEnumerable<ReportModel>> GetConversationsFromUser(string userId);
        Task<IEnumerable<ReportModel>> GetActiveConversations();
        Task<ReportModel> CreateOrUpdateConversation(ReportLogCreationModel conversationLogObj);
        Task<ReportModel> CloseConversation(ReportLogCloseModel conversationCloseObj);
    }
}
