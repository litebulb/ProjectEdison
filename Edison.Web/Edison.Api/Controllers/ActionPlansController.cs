using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Edison.Api.Helpers;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/ActionPlans")]
    public class ActionPlansController : ControllerBase
    {
        private readonly ActionPlanDataManager _actionPlanDataManager;

        public ActionPlansController(ActionPlanDataManager actionPlanDataManager)
        {
            _actionPlanDataManager = actionPlanDataManager;
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [HttpGet("{actionPlanId}")]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> GetActionPlan(Guid actionPlanId)
        {
            ActionPlanModel actionObj = await _actionPlanDataManager.GetActionPlan(actionPlanId);
            return Ok(actionObj);
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [HttpGet]
        [Produces(typeof(IEnumerable<ActionPlanListModel>))]
        public async Task<IActionResult> GetActionPlans()
        {
            IEnumerable<ActionPlanListModel> actionObjs = await _actionPlanDataManager.GetActionPlans();
            return Ok(actionObjs);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> UpdateActionPlan(ActionPlanUpdateModel actionPlanObj)
        {
            var result = await _actionPlanDataManager.UpdateActionPlan(actionPlanObj);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPost]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> CreationActionPlan(ActionPlanCreationModel actionPlanObj)
        {
            var result = await _actionPlanDataManager.CreationActionPlan(actionPlanObj);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpDelete]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> DeleteActionPlan(Guid actionPlanId)
        {
            var result = await _actionPlanDataManager.DeleteActionPlan(actionPlanId);
            return Ok(result);
        }
    }
}
