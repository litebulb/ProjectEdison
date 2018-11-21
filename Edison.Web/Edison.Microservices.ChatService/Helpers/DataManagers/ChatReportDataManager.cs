using Edison.Core.Common.Models;
using System.Threading.Tasks;
using AutoMapper;
using System;
using Edison.Common.Interfaces;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using System.Net;
using Edison.Common.DAO;

namespace Edison.ChatService.Helpers
{
    public class ChatReportDataManager
    {
        private ICosmosDBRepository<ChatReportDAO> _repoChatReports;

        public ChatReportDataManager(ICosmosDBRepository<ChatReportDAO> repoChatReports)
        {
            _repoChatReports = repoChatReports;
        }

        public async Task<ChatReportModel> GetChatReportById(string reportId)
        {
            ChatReportDAO reportEntity = await _repoChatReports.GetItemAsync(reportId);
            return Mapper.Map<ChatReportModel>(reportEntity);
        }

        public async Task<ChatReportModel> GetActiveChatReportFromUser(string userId)
        {
            ChatReportDAO reportEntity = await _repoChatReports.GetItemAsync(p => p.User.Id == userId && p.EndDate.Value == null);
            return Mapper.Map<ChatReportModel>(reportEntity);
        }

        public async Task<IEnumerable<ChatReportModel>> GetChatReportsFromUser(string userId)
        {
            IEnumerable<ChatReportDAO> reportEntities = await _repoChatReports.GetItemsAsync(p => p.User.Id == userId);
            return Mapper.Map<IEnumerable<ChatReportModel>>(reportEntities);
        }

        public async Task<IEnumerable<ChatReportModel>> GetActiveChatReports()
        {
            IEnumerable<ChatReportDAO> reportEntities = await _repoChatReports.GetItemsAsync(p => p.EndDate.Value == null);
            return Mapper.Map<IEnumerable<ChatReportModel>>(reportEntities);
        }

        public async Task<ChatReportModel> CreateOrUpdateChatReport(ChatReportLogCreationModel reportLogObj)
        {
            if (string.IsNullOrEmpty(reportLogObj.User?.Id))
                throw new Exception($"No userId found.");

            ChatReportDAO reportDAO = await _repoChatReports.GetItemAsync(p => p.User.Id == reportLogObj.User.Id && p.EndDate.Value == null);

            //Create
            if (reportDAO == null)
                return await CreateChatReport(reportLogObj);

            //Update
            ReportLogDAOObject reportLogDAO = Mapper.Map<ReportLogDAOObject>(reportLogObj.Message);
            reportDAO.ReportLogs.Add(reportLogDAO);

            try
            {
                await _repoChatReports.UpdateItemAsync(reportDAO);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await CreateOrUpdateChatReport(reportLogObj);
                throw e;
            }

            return Mapper.Map<ChatReportModel>(reportDAO);
        }

        public async Task<ChatReportModel> CreateChatReport(ChatReportLogCreationModel reportLogObj)
        {
            ReportLogDAOObject reportLogDAO = Mapper.Map<ReportLogDAOObject>(reportLogObj.Message);
            ChatReportDAO newReportDAO = new ChatReportDAO()
            {
                ChannelId = reportLogObj.ChannelId,
                ReportLogs = new List<ReportLogDAOObject>() { reportLogDAO },
                User = Mapper.Map<ChatUserDAOObject>(reportLogObj.User)
            };
            newReportDAO.Id = await _repoChatReports.CreateItemAsync(newReportDAO);
            if (string.IsNullOrEmpty(newReportDAO.Id))
                throw new Exception($"An error occured when creating a new chat report: {reportLogObj.User.Id}");

            return Mapper.Map<ChatReportModel>(newReportDAO);
        }

        public async Task<bool> CloseReport(ChatReportLogCloseModel reportCloseObj)
        {
            if (string.IsNullOrEmpty(reportCloseObj.UserId))
                throw new Exception($"No userId found.");

            ChatReportDAO reportDAO = await _repoChatReports.GetItemAsync(p => p.User.Id == reportCloseObj.UserId && p.EndDate.Value == null);

            //If already closed, we do nothing
            if (reportDAO == null)
                return false;

            reportDAO.EndDate = reportCloseObj.EndDate;

            try
            {
                await _repoChatReports.UpdateItemAsync(reportDAO);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    await CloseReport(reportCloseObj);
                throw e;
            }

            return true;
        }
    }
}
