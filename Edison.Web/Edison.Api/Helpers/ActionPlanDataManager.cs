using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using AutoMapper;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for the Action Plan repository
    /// </summary>
    public class ActionPlanDataManager
    {
        private readonly ICosmosDBRepository<ActionPlanDAO> _repoActionPlans;
        private readonly IMapper _mapper;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ActionPlanDataManager(IMapper mapper,
            ICosmosDBRepository<ActionPlanDAO> repoActionPlans)
        {
            _mapper = mapper;
            _repoActionPlans = repoActionPlans;
        }

        /// <summary>
        /// Get action plan from an action plan Id
        /// </summary>
        /// <param name="actionPlanId">Action Plan Id</param>
        /// <returns>ActionPlanModel</returns>
        public async Task<ActionPlanModel> GetActionPlan(Guid actionPlanId)
        {
            ActionPlanDAO plan = await _repoActionPlans.GetItemAsync(actionPlanId);
            return _mapper.Map<ActionPlanModel>(plan);
        }

        /// <summary>
        /// Get all the actions plans
        /// </summary>
        /// <returns>List of Action Plans</returns>
        public async Task<IEnumerable<ActionPlanListModel>> GetActionPlans()
        {
            IEnumerable<ActionPlanDAO> plans = await _repoActionPlans.GetItemsAsync(
            p => p.IsActive,
            p => new ActionPlanDAO()
            {
                Id = p.Id,
                Name = p.Name,
                Color = p.Color,
                Icon = p.Icon,
                Description = p.Description
            }
            );

            return _mapper.Map<IEnumerable<ActionPlanListModel>>(plans);
        }

        /// <summary>
        /// Update an action plan
        /// </summary>
        /// <param name="actionPlanObj">ActionPlanUpdateModel</param>
        /// <returns>ActionPlanModel</returns>
        public async Task<ActionPlanModel> UpdateActionPlan(ActionPlanUpdateModel actionPlanObj)
        {
            ActionPlanDAO plan = await _repoActionPlans.GetItemAsync(actionPlanObj.ActionPlanId);
            if (plan == null)
                throw new Exception($"No action plan found that matches responseid: {actionPlanObj.ActionPlanId}");

            string etag = plan.ETag;
            DateTime creationDate = plan.CreationDate;
            plan = _mapper.Map<ActionPlanDAO>(actionPlanObj);
            plan.ETag = etag;
            plan.CreationDate = creationDate;

            try
            {
                await _repoActionPlans.UpdateItemAsync(plan);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await UpdateActionPlan(actionPlanObj);
                throw e;
            }

            var output = _mapper.Map<ActionPlanModel>(plan);
            return output;
        }

        /// <summary>
        /// Create an action plan
        /// </summary>
        /// <param name="actionPlanObj">ActionPlanCreationModel</param>
        /// <returns>ActionPlanModel</returns>
        public async Task<ActionPlanModel> CreationActionPlan(ActionPlanCreationModel actionPlanObj)
        {
            ActionPlanDAO plan = _mapper.Map<ActionPlanDAO>(actionPlanObj);

            plan.Id = await _repoActionPlans.CreateItemAsync(plan);
            if (_repoActionPlans.IsDocumentKeyNull(plan))
                throw new Exception($"An error occured when creating a new action plan");

            return _mapper.Map<ActionPlanModel>(plan);
        }

        /// <summary>
        /// Delete an action plan
        /// </summary>
        /// <param name="actionPlanId">Id of the action plan</param>
        /// <returns>True if the action plan was removed</returns>
        public async Task<bool> DeleteActionPlan(Guid actionPlanId)
        {
            if (await _repoActionPlans.GetItemAsync(actionPlanId) != null)
                return await _repoActionPlans.DeleteItemAsync(actionPlanId);
            return true;
        }
    }
}
