using AutoMapper;
using Edison.Api.Helpers;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetReports([FromBody]ReportCreationModel reportRequest)
        {
            var result = await _reportDataManager.GetReport(reportRequest);
            return Ok();
        }
    }
}
