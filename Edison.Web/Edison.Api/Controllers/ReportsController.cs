using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Helpers;
using System;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle audit reports
    /// </summary>
    [ApiController]
    [Route("api/Reports")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportDataManager _reportDataManager;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ReportsController(ReportDataManager reportDataManager)
        {
            _reportDataManager = reportDataManager;
        }

        /*public DateTime? MinimumDate { get; set; }
        public DateTime? MaximumDate { get; set; }
        public ReportCreationType Type { get; set; }
        public string Filename { get; set; }*/


        /// <summary>
        /// Get reports based on minimum date, maximum date and a type of report
        /// </summary>
        /// <param name="reportRequest">ReportCreationModel</param>
        /// <returns>XLSX Data</returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Admin)]
        public async Task<IActionResult> GetReports(DateTime? minimumDate, DateTime? maximumDate, ReportCreationType type, string filename)
        {

            var result = await _reportDataManager.GetReport(new ReportCreationModel() {
                 Filename = filename, MaximumDate = maximumDate, MinimumDate = minimumDate, Type = type
            });
            if(result != null && result.Length > 0)
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{(string.IsNullOrEmpty(filename) ? "report" : filename)}.xlsx");
            if (result != null && result.Length == 0)
                return Ok();
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
