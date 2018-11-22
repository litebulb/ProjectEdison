using AutoMapper;
using Edison.Api.Helpers;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Threading.Tasks;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/Reports")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportDataManager _reportDataManager;

        public ReportsController(ReportDataManager reportDataManager)
        {
            _reportDataManager = reportDataManager;
        }

        [HttpPost]
        public async Task<IActionResult> GetReports()
        //public async Task<IActionResult> GetReports([FromBody]ReportCreationModel reportRequest)
        {
            ReportCreationModel reportRequest = new ReportCreationModel() { };
            var result = await _reportDataManager.GetReport(reportRequest);
            if(result != null && result.Length > 0)
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
            if (result != null && result.Length == 0)
                return Ok();
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
