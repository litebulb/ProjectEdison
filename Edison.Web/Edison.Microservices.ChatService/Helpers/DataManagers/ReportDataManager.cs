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
    public class ReportDataManager
    {
        private ICosmosDBRepository<ReportDAO> _repoReports;

        public ReportDataManager(ICosmosDBRepository<ReportDAO> repoReports)
        {
            _repoReports = repoReports;
        }

        public async Task<ReportModel> GetReportById(string reportId)
        {
            ReportDAO reportEntity = await _repoReports.GetItemAsync(reportId);
            return Mapper.Map<ReportModel>(reportEntity);
        }

        public async Task<ReportModel> GetActiveReportFromUser(string userId)
        {
            ReportDAO reportEntity = await _repoReports.GetItemAsync(p => p.User.Id == userId && p.EndDate.Value == null);
            return Mapper.Map<ReportModel>(reportEntity);
        }

        public async Task<IEnumerable<ReportModel>> GetReportsFromUser(string userId)
        {
            IEnumerable<ReportDAO> reportEntities = await _repoReports.GetItemsAsync(p => p.User.Id == userId);
            return Mapper.Map<IEnumerable<ReportModel>>(reportEntities);
        }

        public async Task<IEnumerable<ReportModel>> GetActiveReports()
        {
            IEnumerable<ReportDAO> reportEntities = await _repoReports.GetItemsAsync(p => p.EndDate.Value == null);
            return Mapper.Map<IEnumerable<ReportModel>>(reportEntities);
        }

        public async Task<ReportModel> CreateOrUpdateReport(ReportLogCreationModel reportLogObj)
        {
            if (string.IsNullOrEmpty(reportLogObj.User?.Id))
                throw new Exception($"No userId found.");

            ReportDAO reportDAO = await _repoReports.GetItemAsync(p => p.User.Id == reportLogObj.User.Id && p.EndDate.Value == null);

            //Create
            if (reportDAO == null)
                return await CreateReport(reportLogObj);

            //Update
            ReportLogDAOObject reportLogDAO = Mapper.Map<ReportLogDAOObject>(reportLogObj.Message);
            reportDAO.ReportLogs.Add(reportLogDAO);

            try
            {
                await _repoReports.UpdateItemAsync(reportDAO);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await CreateOrUpdateReport(reportLogObj);
                throw e;
            }

            return Mapper.Map<ReportModel>(reportDAO);
        }

        public async Task<ReportModel> CreateReport(ReportLogCreationModel reportLogObj)
        {
            ReportLogDAOObject reportLogDAO = Mapper.Map<ReportLogDAOObject>(reportLogObj.Message);
            ReportDAO newReportDAO = new ReportDAO()
            {
                ChannelId = reportLogObj.ChannelId,
                ReportLogs = new List<ReportLogDAOObject>() { reportLogDAO },
                User = Mapper.Map<ChatUserDAOObject>(reportLogObj.User)
            };
            newReportDAO.Id = await _repoReports.CreateItemAsync(newReportDAO);
            if (string.IsNullOrEmpty(newReportDAO.Id))
                throw new Exception($"An error occured when creating a new report: {reportLogObj.User.Id}");

            return Mapper.Map<ReportModel>(newReportDAO);
        }

        public async Task<bool> CloseReport(ReportLogCloseModel reportCloseObj)
        {
            if (string.IsNullOrEmpty(reportCloseObj.UserId))
                throw new Exception($"No userId found.");

            ReportDAO reportDAO = await _repoReports.GetItemAsync(p => p.User.Id == reportCloseObj.UserId && p.EndDate.Value == null);

            //If already closed, we do nothing
            if (reportDAO == null)
                return false;

            reportDAO.EndDate = reportCloseObj.EndDate;

            try
            {
                await _repoReports.UpdateItemAsync(reportDAO);
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
