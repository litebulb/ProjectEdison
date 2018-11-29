using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Drawing;
using Microsoft.Extensions.Options;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.DAO;
using Edison.Api.Config;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for Audit and report generation
    /// </summary>
    public class ReportDataManager
    {
        private readonly WebApiOptions _config;
        private readonly ICosmosDBRepository<ResponseDAO> _repoResponses;
        private readonly ICosmosDBRepository<EventClusterDAO> _repoEventClusters;
        private readonly ICosmosDBRepository<ChatReportDAO> _repoChatReports;
        private readonly Dictionary<string, int> _mappingUserColumns;
        private readonly Dictionary<string, int> _mappingConversationColumns;
        private readonly Dictionary<string, int> _mappingEventColumns;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ReportDataManager(IOptions<WebApiOptions> config,
            ICosmosDBRepository<ResponseDAO> repoResponses, 
            ICosmosDBRepository<EventClusterDAO> repoEventClusters,
            ICosmosDBRepository<ChatReportDAO> repoChatReports)
        {
            _config = config.Value;
            _repoResponses = repoResponses;
            _repoEventClusters = repoEventClusters;
            _repoChatReports = repoChatReports;

            _mappingConversationColumns = new Dictionary<string, int>();
            _mappingUserColumns = new Dictionary<string, int>();
            _mappingEventColumns = new Dictionary<string, int>();
            SetupMappingTable();
        }

        /// <summary>
        /// Generate a report
        /// </summary>
        /// <param name="reportRequest">ReportCreationModel</param>
        /// <returns>XLSX data</returns>
        public async Task<byte[]> GetReport(ReportCreationModel reportRequest)
        {
            byte[] export = null;

            using (MemoryStream stream = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();

                if ((reportRequest.Type & ReportCreationType.Events) != 0)
                    await GenerateEventsReport(workbook, reportRequest.MinimumDate, reportRequest.MaximumDate);

                if ((reportRequest.Type & ReportCreationType.Conversations) != 0)
                {
                    //Handle Conversations
                }

                //Handle Responses
                if ((reportRequest.Type & ReportCreationType.Responses) != 0)
                    await GenerateResponsesReport(workbook, reportRequest.MinimumDate, reportRequest.MaximumDate);

                workbook.Write(stream);
                export = stream.ToArray();
            }

            return export;
        }

        /// <summary>
        /// Generate a report of events
        /// </summary>
        /// <param name="workbook">Workbook</param>
        /// <param name="requestedMinDate">Filter minimum date</param>
        /// <param name="requestedMaxDate">Filter maximum date</param>
        /// <returns>Task</returns>
        private async Task GenerateEventsReport(IWorkbook workbook, DateTime? requestedMinDate, DateTime? requestedMaxDate)
        {
            //Retrieve responses
            var eventClusters = await GetListBetweenDates(_repoEventClusters, requestedMinDate, requestedMaxDate);
            if (eventClusters != null)
            {
                ISheet eventClustersEventSheet = workbook.CreateSheet($"Events");
                eventClustersEventSheet.DefaultColumnWidth = GetColumnWidth(_config.ReportConfiguration.DefaultWidth);
                GenerateEventsReport(eventClustersEventSheet, 0, eventClusters);
            }
        }

        /// <summary>
        /// Generate a report of responses
        /// </summary>
        /// <param name="workbook">Workbook</param>
        /// <param name="requestedMinDate">Filter minimum date</param>
        /// <param name="requestedMaxDate">Filter maximum date</param>
        /// <returns>Task</returns>
        private async Task GenerateResponsesReport(IWorkbook workbook, DateTime? requestedMinDate, DateTime? requestedMaxDate)
        {
            //Retrieve responses
            var responses = await GetListBetweenDates(_repoResponses, requestedMinDate, requestedMaxDate);
            if (responses != null)
            {
                //Figuring out the latest enddate. If one enddate = null, then latest endate = current date
                DateTime maxDate = DateTime.UtcNow;
                if (responses.Where(p => p.EndDate.Value == null) == null)
                    maxDate = responses.Max(p => p.EndDate).Value;

                //Retrieve all associated event clusters and chat reports
                var eventClusters = await GetListBetweenDates(_repoEventClusters, requestedMinDate, maxDate);
                var chatReports = await GetListBetweenDates(_repoChatReports, requestedMinDate, maxDate);

                //Enum responses
                foreach (var response in responses)
                {
                    ISheet responseEventSheet = workbook.CreateSheet($"{response.ActionPlan.Name} - Events - {response.Id}");
                    responseEventSheet.DefaultColumnWidth = GetColumnWidth(_config.ReportConfiguration.DefaultWidth);
                    IEnumerable<EventClusterDAO> responseEventClusters = eventClusters.Where(e => response.EventClusterIds.Any(r => r.ToString() == e.Id));
                    GenerateResponseHeaderReport(workbook, responseEventSheet, 0, response);
                    GenerateEventsReport(responseEventSheet, responseEventSheet.LastRowNum + 1, responseEventClusters);

                    if (response.ActionPlan.AcceptSafeStatus && response.SafeUsers != null && response.SafeUsers.Count > 0)
                    {
                        ISheet userSafeListEventSheet = workbook.CreateSheet($"{response.ActionPlan.Name} - User SafeList - {response.Id}");
                        userSafeListEventSheet.DefaultColumnWidth = GetColumnWidth(_config.ReportConfiguration.DefaultWidth);
                        GenerateResponseHeaderReport(workbook, userSafeListEventSheet, 0, response);
                        GenerateUserSafeListReport(userSafeListEventSheet, userSafeListEventSheet.LastRowNum + 1, response.SafeUsers);
                    }
                }

            }
        }

        /// <summary>
        /// Generate the header of a response sheet
        /// </summary>
        /// <param name="workbook">Workbook</param>
        /// <param name="sheet">Sheet</param>
        /// <param name="rowStartIndex">Starting row</param>
        /// <param name="response">Response object</param>
        private void GenerateResponseHeaderReport(IWorkbook workbook, ISheet sheet, int rowStartIndex, ResponseDAO response)
        {
            List<ReportResponseHeaderRowOptions> responseHeaders = _config.ReportConfiguration.ResponseHeader;
            if (responseHeaders == null)
                return;

            foreach (var responseHeaderRow in responseHeaders)
            {
                IRow row = sheet.CreateRow(rowStartIndex + responseHeaderRow.RowIndex);
                foreach (var responseHeaderColumn in responseHeaderRow.Columns)
                {
                    var value = GetResponseHeaderValue(responseHeaderColumn.Value, response);
                    if(value is double)
                        SetCellValue(row, responseHeaderColumn.ColumnIndex, value, GetStyle(workbook, responseHeaderColumn.Style), ReportDataType.Double);
                    else
                        SetCellValue(row, responseHeaderColumn.ColumnIndex, value, GetStyle(workbook, responseHeaderColumn.Style), ReportDataType.Text);
                }

            }
        }

        /// <summary>
        /// Generate the events report
        /// </summary>
        /// <param name="sheet">Sheet</param>
        /// <param name="rowStartIndex">Starting row</param>
        /// <param name="eventClusters">List of Event Clusters</param>
        private void GenerateEventsReport(ISheet sheet, int rowStartIndex, IEnumerable<EventClusterDAO> eventClusters)
        {
            Dictionary<string, XSSFCellStyle> rowCellStyles = SetupRowStyles(sheet.Workbook, _config.ReportConfiguration.EventsReport);
            SetupColumns(sheet, rowStartIndex, _config.ReportConfiguration.EventsReport);
            int rowIndex = rowStartIndex + 1;

            //Enum event clusters
            foreach (var eventCluster in eventClusters)
            {
                //Enum events
                foreach (var eventObj in eventCluster.Events)
                {
                    IRow row = sheet.CreateRow(rowIndex);

                    SetCellValue(row, _mappingEventColumns["eventClusterId"], eventCluster.Id.ToString(), rowCellStyles["eventClusterId"]);
                    SetCellValue(row, _mappingEventColumns["deviceId"], eventCluster.Device?.DeviceId.ToString(), rowCellStyles["deviceId"]);
                    SetCellValue(row, _mappingEventColumns["eventDate"], eventObj.Date.ToOADate(), rowCellStyles["eventDate"], ReportDataType.Date);
                    SetCellValue(row, _mappingEventColumns["eventType"], eventCluster.EventType, rowCellStyles["eventType"]);
                    SetCellValue(row, _mappingEventColumns["deviceType"], eventCluster.Device?.DeviceType, rowCellStyles["deviceType"]);
                    SetCellValue(row, _mappingEventColumns["deviceName"], eventCluster.Device?.Name, rowCellStyles["deviceName"]);
                    SetCellValue(row, _mappingEventColumns["deviceLocation1"], eventCluster.Device?.Location1, rowCellStyles["deviceLocation1"]);
                    SetCellValue(row, _mappingEventColumns["deviceLocation2"], eventCluster.Device?.Location2, rowCellStyles["deviceLocation2"]);
                    SetCellValue(row, _mappingEventColumns["deviceLocation3"], eventCluster.Device?.Location3, rowCellStyles["deviceLocation3"]);
                    SetCellValue(row, _mappingEventColumns["deviceGeolocationLon"], eventCluster.Device?.Geolocation?.Longitude, rowCellStyles["deviceGeolocationLon"], ReportDataType.Double);
                    SetCellValue(row, _mappingEventColumns["deviceGeolocationLat"], eventCluster.Device?.Geolocation?.Latitude, rowCellStyles["deviceGeolocationLat"], ReportDataType.Double);
                    SetCellValue(row, _mappingEventColumns["eventMetadata"], string.Join(", ", eventObj.Metadata.Select(x => x.Key + ": " + x.Value)), rowCellStyles["eventMetadata"]);

                    rowIndex++;
                }
            }
        }

        /// <summary>
        /// Generate a user safe list
        /// </summary>
        /// <param name="sheet">Sheet</param>
        /// <param name="rowStartIndex">Starting row</param>
        /// <param name="userSafeList">List of user safe</param>
        private void GenerateUserSafeListReport(ISheet sheet, int rowStartIndex, IEnumerable<string> userSafeList)
        {
            Dictionary<string, XSSFCellStyle> rowCellStyles = SetupRowStyles(sheet.Workbook, _config.ReportConfiguration.UsersReport);
            SetupColumns(sheet, rowStartIndex, _config.ReportConfiguration.UsersReport);
            int rowIndex = rowStartIndex + 1;

            //Enum event clusters
            foreach (var userEmail in userSafeList)
            {
                //Enum events
                    IRow row = sheet.CreateRow(rowIndex);

                    SetCellValue(row, _mappingUserColumns["userEmail"], userEmail, rowCellStyles["userEmail"]);

                    rowIndex++;
            }
        }

        #region Utility Methods
        /// <summary>
        /// Set the value of a cell
        /// </summary>
        /// <param name="row">Row</param>
        /// <param name="cellIndex">Index of the cell in the sheet</param>
        /// <param name="value">Value of the cell</param>
        /// <param name="cellStyle">Style of the cell</param>
        /// <param name="dataType">DataType</param>
        private void SetCellValue(IRow row, int cellIndex, object value, XSSFCellStyle cellStyle, ReportDataType dataType = ReportDataType.Text)
        {
            ICell cell = row.CreateCell(cellIndex);

            switch (dataType)
            {
                case ReportDataType.Date:
                case ReportDataType.Double:
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue((double)value);
                    break;
                case ReportDataType.Integer:
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue((int)value);
                    break;
                case ReportDataType.RichText:
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue((IRichTextString)value);
                    break;
                default:
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue((string)value);
                    break;
            }
            cell.CellStyle = cellStyle;
        }

        /// <summary>
        /// Generate columns and styles for a sheet
        /// </summary>
        /// <param name="sheet">Sheet</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="columnsOptions">List of ReportColumnOptions</param>
        private void SetupColumns(ISheet sheet, int rowIndex, IEnumerable<ReportColumnOptions> columnsOptions)
        {
            IWorkbook workbook = sheet.Workbook;
            IRow row = sheet.CreateRow(rowIndex);

            foreach (var columnOptions in columnsOptions.OrderBy(p => p.ColumnIndex))
            {
                //Set up column style
                XSSFCellStyle cellStyle = GetStyle(workbook, columnOptions.HeaderStyle);
                //Set Hidden state
                sheet.SetColumnHidden(columnOptions.ColumnIndex, columnOptions.IsHidden);
                //Set Column Width
                sheet.SetColumnWidth(columnOptions.ColumnIndex, GetColumnWidth(columnOptions.Width));

                //Create Cell
                ICell cell = row.CreateCell(columnOptions.ColumnIndex);
                cell.SetCellValue(columnOptions.HeaderName);
                cell.CellStyle = cellStyle;
            }

            //Add freezing Pane + Filtering
            sheet.CreateFreezePane(0, rowIndex + 1);
            sheet.SetAutoFilter(new CellRangeAddress(rowIndex, rowIndex, 0, columnsOptions.Count() - 1));
        }

        /// <summary>
        /// Get a specific cell style
        /// </summary>
        /// <param name="workbook">Workbook</param>
        /// <param name="reportStyle">ReportStyleCell</param>
        /// <returns>XSSFCellStyle</returns>
        private XSSFCellStyle GetStyle(IWorkbook workbook, ReportStyleCell reportStyle)
        {
            ICreationHelper creationHelper = workbook.GetCreationHelper();

            //Set up column style
            XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            if (reportStyle == null)
                return cellStyle;

            XSSFFont font = (XSSFFont)workbook.CreateFont();
            font.Boldweight = (short)reportStyle.FontWeight;

            //DataFormat
            if (!string.IsNullOrEmpty(reportStyle.DataFormat))
            {
                cellStyle.SetDataFormat(creationHelper.CreateDataFormat().GetFormat(reportStyle.DataFormat));
                cellStyle.Alignment = reportStyle.HorizontalAlignment;
            }

            //BackgroundColor
            if (!string.IsNullOrEmpty(reportStyle.BackgroundColor))
            {
                XSSFColor backgroundColor = new XSSFColor(Color.FromName(reportStyle.BackgroundColor));
                cellStyle.FillPattern = FillPattern.SolidForeground;
                cellStyle.SetFillForegroundColor(backgroundColor);
            }
            //Foreground Color
            if (!string.IsNullOrEmpty(reportStyle.FontColor))
            {
                XSSFColor foregroundColor = new XSSFColor(Color.FromName(reportStyle.FontColor));
                font.SetColor(foregroundColor);
            }
            //Wrap Text
            cellStyle.SetFont(font);
            cellStyle.WrapText = reportStyle.WrapText;

            return cellStyle;
        }

        /// <summary>
        /// One time setup of the maping time, to map a column name to an index
        /// </summary>
        private void SetupMappingTable()
        {
            _mappingConversationColumns.Clear();
            _mappingUserColumns.Clear();
            _mappingEventColumns.Clear();

            if (_config.ReportConfiguration != null)
            {
                if (_config.ReportConfiguration.ConversationsReport != null)
                    foreach (var conversationColumn in _config.ReportConfiguration.ConversationsReport)
                        _mappingConversationColumns.Add(conversationColumn.Name, conversationColumn.ColumnIndex);

                if (_config.ReportConfiguration.UsersReport != null)
                    foreach (var userColumn in _config.ReportConfiguration.UsersReport)
                        _mappingUserColumns.Add(userColumn.Name, userColumn.ColumnIndex);

                if (_config.ReportConfiguration.EventsReport != null)
                    foreach (var eventColumn in _config.ReportConfiguration.EventsReport)
                        _mappingEventColumns.Add(eventColumn.Name, eventColumn.ColumnIndex);
            }
        }

        /// <summary>
        /// Generate rows and styles for a sheet
        /// </summary>
        /// <param name="workbook">Workbook</param>
        /// <param name="columnsOptions">List of ReportColumnOptions</param>
        /// <returns>Dictionary of XSSFCellStyle</returns>
        private Dictionary<string, XSSFCellStyle> SetupRowStyles(IWorkbook workbook, IEnumerable<ReportColumnOptions> columnsOptions)
        {
            if (columnsOptions == null)
                return new Dictionary<string, XSSFCellStyle>();

            Dictionary<string, XSSFCellStyle> output = new Dictionary<string, XSSFCellStyle>();
            foreach (var columnOptions in columnsOptions)
            {
                //Get style
                XSSFCellStyle cellStyle = GetStyle(workbook, columnOptions.RowStyle);
                //Output
                output.Add(columnOptions.Name, cellStyle);
            }

            return output;
        }

        /// <summary>
        /// Get a list of data object from one starting date to another
        /// </summary>
        /// <typeparam name="T">Type of the repository</typeparam>
        /// <param name="repository">ICosmosDBRepository object</param>
        /// <param name="minDate">Minimum date</param>
        /// <param name="maxDate">Maximum date</param>
        /// <returns>List of repository objects</returns>
        private async Task<IEnumerable<T>> GetListBetweenDates<T>(ICosmosDBRepository<T> repository, DateTime? minDate, DateTime? maxDate) where T : IEntityDAO
        {
            var results = await repository.GetItemsAsync(p =>
            (minDate == null || p.CreationDate >= minDate.Value) &&
            (maxDate == null || p.CreationDate <= maxDate.Value));
            return results;
        }

        /// <summary>
        /// Get a calculated value of a row size
        /// </summary>
        /// <param name="width">Size in pixels</param>
        /// <returns>Actual size</returns>
        private int GetColumnWidth(int width)
        {
            if (width > 254)
                return 65280; // Maximum allowed column width. 
            if (width > 1)
            {
                int floor = (int)(Math.Floor(((double)width) / 5));
                int factor = (30 * floor);
                int value = 450 + factor + ((width - 1) * 250);
                return value;
            }
            else
                return 450; // default to column size 1 if zero, one or negative number is passed. 
        }

        /// <summary>
        /// Map a header key to its actual value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="response">Response object</param>
        /// <returns>Value</returns>
        private object GetResponseHeaderValue(string key, ResponseDAO response)
        {
            switch (key)
            {
                case "{RESPONSETYPE}":
                    return response.ActionPlan.Name;
                case "{RESPONSEDESCRIPTION}":
                    return response.ActionPlan.Description;
                case "{PRIMARYRADIUS}":
                    return response.ActionPlan.PrimaryRadius.ToString();
                case "{SECONDARYRADIUS}":
                    return response.ActionPlan.SecondaryRadius.ToString();
                case "{PRIMARYEVENTCLUSTERID}":
                    return response.PrimaryEventClusterId.ToString();
                case "{LONGITUDE}":
                    return response.Geolocation?.Longitude.ToString();
                case "{LATITUDE}":
                    return response.Geolocation?.Latitude.ToString();
                case "{CREATIONDATE}":
                    return response.CreationDate.ToOADate();
                case "{ENDDATE}":
                    if (response.EndDate == null)
                        return "ONGOING";
                    return response.EndDate.Value.ToOADate();
            }
            return key;
        }
        #endregion
    }
}
