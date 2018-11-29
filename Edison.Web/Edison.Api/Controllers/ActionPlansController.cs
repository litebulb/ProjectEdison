using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle CRUD operation on Action Plans
    /// </summary>
    [ApiController]
    [Route("api/ActionPlans")]
    public class ActionPlansController : ControllerBase
    {
        private readonly ActionPlanDataManager _actionPlanDataManager;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ActionPlansController(ActionPlanDataManager actionPlanDataManager)
        {
            _actionPlanDataManager = actionPlanDataManager;
        }

        /// <summary>
        /// Get action plan from an action plan Id
        /// </summary>
        /// <param name="actionPlanId">Action Plan Id</param>
        /// <returns>ActionPlanModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpGet("{actionPlanId}")]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> GetActionPlan(Guid actionPlanId)
        {
            ActionPlanModel actionObj = await _actionPlanDataManager.GetActionPlan(actionPlanId);
            return Ok(actionObj);
        }

        /// <summary>
        /// Get all the actions plans
        /// </summary>
        /// <returns>List of Action Plans</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpGet]
        [Produces(typeof(IEnumerable<ActionPlanListModel>))]
        public async Task<IActionResult> GetActionPlans()
        {
            IEnumerable<ActionPlanListModel> actionObjs = await _actionPlanDataManager.GetActionPlans();
            return Ok(actionObjs);
        }

        /// <summary>
        /// Update an action plan
        /// </summary>
        /// <param name="actionPlanObj">ActionPlanUpdateModel</param>
        /// <returns>ActionPlanModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> UpdateActionPlan(ActionPlanUpdateModel actionPlanObj)
        {
            var result = await _actionPlanDataManager.UpdateActionPlan(actionPlanObj);
            return Ok(result);
        }

        /// <summary>
        /// Create an action plan
        /// </summary>
        /// <param name="actionPlanObj">ActionPlanCreationModel</param>
        /// <returns>ActionPlanModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPost]
        [Produces(typeof(ActionPlanModel))]
        public async Task<IActionResult> CreationActionPlan(ActionPlanCreationModel actionPlanObj)
        {
            var result = await _actionPlanDataManager.CreationActionPlan(actionPlanObj);
            return Ok(result);
        }

        /// <summary>
        /// Delete an action plan
        /// </summary>
        /// <param name="actionPlanId">Id of the action plan</param>
        /// <returns>True if the action plan was removed</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpDelete]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> DeleteActionPlan(Guid actionPlanId)
        {
            var result = await _actionPlanDataManager.DeleteActionPlan(actionPlanId);
            return Ok(result);
        }
    }
}
