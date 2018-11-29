using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Helpers;

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

        public ReportsController(ReportDataManager reportDataManager)
        {
            _reportDataManager = reportDataManager;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Admin)]
        public async Task<IActionResult> GetReports([FromBody]ReportCreationModel reportRequest)
        {

            var result = await _reportDataManager.GetReport(reportRequest);
            if(result != null && result.Length > 0)
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{(string.IsNullOrEmpty(reportRequest.Filename) ? "report" : reportRequest.Filename)}.xlsx");
            if (result != null && result.Length == 0)
                return Ok();
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
