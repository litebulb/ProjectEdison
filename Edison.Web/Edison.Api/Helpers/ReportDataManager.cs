using Edison.Core.Common.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Edison.Common.Interfaces;
using Edison.Common.DAO;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Drawing;
using Edison.Api.Config;

namespace Edison.Api.Helpers
{
    public class ReportDataManager
    {
        private readonly WebApiOptions _config;
        private readonly ICosmosDBRepository<ResponseDAO> _repoResponses;
        private readonly ICosmosDBRepository<EventClusterDAO> _repoEventClusters;
        private readonly ICosmosDBRepository<ChatReportDAO> _repoChatReports;
        private readonly Dictionary<string, int> _mappingResponseColumns;
        private readonly Dictionary<string, int> _mappingEventColumns;

        public ReportDataManager(IOptions<WebApiOptions> config,
            ICosmosDBRepository<ResponseDAO> repoResponses, 
            ICosmosDBRepository<EventClusterDAO> repoEventClusters,
            ICosmosDBRepository<ChatReportDAO> repoChatReports)
        {
            _config = config.Value;
            _repoResponses = repoResponses;
            _repoEventClusters = repoEventClusters;
            _repoChatReports = repoChatReports;

            _mappingResponseColumns = new Dictionary<string, int>();
            _mappingEventColumns = new Dictionary<string, int>();
            SetupMappingTable();
        }
        
        public async Task<byte[]> GetReport(ReportCreationModel reportRequest)
        {
            reportRequest.MinimumDate = DateTime.UtcNow.AddDays(-30);
            reportRequest.MaximumDate = DateTime.UtcNow.AddDays(30);

            byte[] export = null;

            //Retrieve responses
            var responses = await GetListBetweenDates(_repoResponses, reportRequest.MinimumDate, reportRequest.MaximumDate);
            if (responses != null)
            {
                //Figuring out the latest enddate. If one enddate = null, then latest endate = current date
                DateTime maxDate = DateTime.UtcNow;
                if (responses.Where(p => p.EndDate.Value == null) == null)
                    maxDate = responses.Max(p => p.EndDate).Value;

                //Retrieve all associated event clusters and chat reports
                var eventClusters = await GetListBetweenDates(_repoEventClusters, reportRequest.MinimumDate, maxDate);
                var chatReports = await GetListBetweenDates(_repoChatReports, reportRequest.MinimumDate, maxDate);

                //Generating XLSX
                using (MemoryStream stream = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ICreationHelper creationHelper = workbook.GetCreationHelper();

                    //Enum responses
                    foreach (var response in responses)
                    {
                        ISheet sheet = workbook.CreateSheet($"{response.ActionPlan.Name} - Events - {response.Id}");
                        SetupColumns(sheet, _config.ReportConfiguration.ResponsesReport);
                        int rowIndex = 0;

                        //Enum event clusters
                        IEnumerable<EventClusterDAO> responseEventClusters = eventClusters.Where(e => response.EventClusterIds.Any(r => r.ToString() == e.Id));
                        foreach(var eventCluster in responseEventClusters)
                        {
                            //Enum events
                            foreach (var eventObj in eventCluster.Events)
                            {
                                IRow row = sheet.CreateRow(rowIndex);

                                SetCellValue(row, _mappingResponseColumns["eventClusterId"], eventCluster.Id.ToString());
                                SetCellValue(row, _mappingResponseColumns["deviceId"], eventCluster.Device?.DeviceId.ToString());
                                SetCellValue(row, _mappingResponseColumns["eventDate"], eventObj.Date.ToString());
                                SetCellValue(row, _mappingResponseColumns["eventType"], eventCluster.EventType);
                                SetCellValue(row, _mappingResponseColumns["deviceType"], eventCluster.Device?.DeviceType);
                                SetCellValue(row, _mappingResponseColumns["deviceName"], eventCluster.Device?.Name);
                                SetCellValue(row, _mappingResponseColumns["deviceLocation1"], eventCluster.Device?.Location1);
                                SetCellValue(row, _mappingResponseColumns["deviceLocation2"], eventCluster.Device?.Location2);
                                SetCellValue(row, _mappingResponseColumns["deviceLocation3"], eventCluster.Device?.Location3);
                                SetCellValue(row, _mappingResponseColumns["deviceGeolocationLon"], eventCluster.Device?.Geolocation?.Longitude, CellType.Numeric);
                                SetCellValue(row, _mappingResponseColumns["deviceGeolocationLat"], eventCluster.Device?.Geolocation?.Latitude, CellType.Numeric);
                                SetCellValue(row, _mappingResponseColumns["eventMetadata"], string.Join(", ", eventObj.Metadata.ToString()));
                                if (eventCluster.EventType == "message" && eventCluster.EventCount > 0 && eventCluster.Events[0].Metadata.ContainsKey("userId"))
                                {
                                    var chatReport = chatReports.Where(p => p.User.Id == eventCluster.Events[0].Metadata["chatReportId"].ToString()).First();
                                    if(chatReport != null)
                                        SetCellValue(row, _mappingResponseColumns["chatReportId"], creationHelper.CreateRichTextString(GenerateChatUserReport(chatReport)));
                                }

                                rowIndex++;
                            }
                        }
                    }

                    workbook.Write(stream);
                    export = stream.ToArray();
                }
            }
            return export;
        }

        private void SetupColumns(ISheet sheet, IEnumerable<ReportColumnOptions> columnsOptions)
        {
            IWorkbook workboot = sheet.Workbook;
            IRow row = sheet.CreateRow(0);

            foreach (var columnOptions in columnsOptions.OrderBy(p => p.ColumnIndex))
            {
                //Set up column style
                XSSFCellStyle cellStyle = (XSSFCellStyle)workboot.CreateCellStyle();

                //BackgroundColor
                if(!string.IsNullOrEmpty(columnOptions.BackgroundColor))
                    cellStyle.SetFillBackgroundColor(new XSSFColor(Color.FromName(columnOptions.BackgroundColor)));
                //Foreground Color
                if (!string.IsNullOrEmpty(columnOptions.Color))
                    cellStyle.SetFillForegroundColor(new XSSFColor(Color.FromName(columnOptions.Color)));
                cellStyle.IsHidden = columnOptions.IsHidden;
                sheet.SetDefaultColumnStyle(columnOptions.ColumnIndex, cellStyle);
                sheet.SetColumnWidth(columnOptions.ColumnIndex, columnOptions.Width);

                //Set up 
                ICell cell = row.CreateCell(columnOptions.ColumnIndex);
                cell.SetCellValue(columnOptions.DisplayName);
            }
        }

        private void SetCellValue(IRow row, int cellIndex, object value, CellType cellType = CellType.String)
        {
            ICell cell = row.CreateCell(cellIndex);
            cell.SetCellType(cellType);
            if(value is string)
                cell.SetCellValue((string)value);
            else if (value is double)
                cell.SetCellValue((double)value);
            else if (value is int)
                cell.SetCellValue((double)value);
            else if (value is IRichTextString)
                cell.SetCellValue((IRichTextString)value);
        }

        private async Task<IEnumerable<T>> GetListBetweenDates<T>(ICosmosDBRepository<T> repository, DateTime? minDate, DateTime? maxDate) where T : IEntityDAO
        {
            var results = await repository.GetItemsAsync(p =>
            (minDate == null || p.CreationDate >= minDate.Value) &&
            (maxDate == null || p.CreationDate <= maxDate.Value));
            return results;
        }

        private void SetupMappingTable()
        {
            _mappingResponseColumns.Clear();
            _mappingEventColumns.Clear();

            if (_config.ReportConfiguration != null)
            {
                if(_config.ReportConfiguration.ResponsesReport != null)
                    foreach (var responseColumn in _config.ReportConfiguration.ResponsesReport)
                        _mappingResponseColumns.Add(responseColumn.Name, responseColumn.ColumnIndex);
                if (_config.ReportConfiguration.EventsReport != null)
                    foreach (var eventColumn in _config.ReportConfiguration.EventsReport)
                        _mappingEventColumns.Add(eventColumn.Name, eventColumn.ColumnIndex);
            }
        }

        private string GenerateChatUserReport(ChatReportDAO chatReport)
        {
            return string.Empty;
        }
    }
}
