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
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [ApiController]
    [Route("api/ActionPlans")]
    public class ActionPlansController : ControllerBase
    {
        private readonly ActionPlanDataManager _actionPlanDataManager;

        public ActionPlansController(ActionPlanDataManager actionPlanDataManager)
        {
            _actionPlanDataManager = actionPlanDataManager;
        }

        [HttpGet("{actionPlanId}")]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> GetActionPlan(Guid actionPlanId)
        {
            ActionPlanModel actionObj = await _actionPlanDataManager.GetActionPlan(actionPlanId);
            return Ok(actionObj);
        }

        [HttpGet]
        [Produces(typeof(IEnumerable<ActionPlanListModel>))]
        public async Task<IActionResult> GetActionPlans()
        {
            IEnumerable<ActionPlanListModel> actionObjs = await _actionPlanDataManager.GetActionPlans();
            return Ok(actionObjs);
        }

        [HttpPut]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> UpdateActionPlan(ActionPlanUpdateModel actionPlanObj)
        {
            var result = await _actionPlanDataManager.UpdateActionPlan(actionPlanObj);
            return Ok(result);
        }

        [HttpPost]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> CreationActionPlan(ActionPlanCreationModel actionPlanObj)
        {
            var result = await _actionPlanDataManager.CreationActionPlan(actionPlanObj);
            return Ok(result);
        }

        [HttpDelete]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> DeleteActionPlan(Guid actionPlanId)
        {
            var result = await _actionPlanDataManager.DeleteActionPlan(actionPlanId);
            return Ok(result);
        }
    }
}
