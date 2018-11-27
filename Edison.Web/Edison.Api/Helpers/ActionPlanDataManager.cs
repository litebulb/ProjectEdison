using Edison.Api.Config;
using Edison.Core.Common.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using System;
using Microsoft.Azure.Documents;
using System.Net;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    public class ActionPlanDataManager
    {
        private readonly ICosmosDBRepository<ActionPlanDAO> _repoActionPlans;
        private readonly IMapper _mapper;

        public ActionPlanDataManager(IMapper mapper,
            ICosmosDBRepository<ActionPlanDAO> repoActionPlans)
        {
            _mapper = mapper;
            _repoActionPlans = repoActionPlans;
        }

        #region Action Plans
        public async Task<ActionPlanModel> GetActionPlan(Guid actionPlanId)
        {
            ActionPlanDAO plan = await _repoActionPlans.GetItemAsync(actionPlanId);
            return _mapper.Map<ActionPlanModel>(plan);
        }

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

        public async Task<ActionPlanModel> CreationActionPlan(ActionPlanCreationModel actionPlanObj)
        {
            ActionPlanDAO plan = _mapper.Map<ActionPlanDAO>(actionPlanObj);

            plan.Id = await _repoActionPlans.CreateItemAsync(plan);
            if (_repoActionPlans.IsDocumentKeyNull(plan))
                throw new Exception($"An error occured when creating a new action plan");

            return _mapper.Map<ActionPlanModel>(plan);
        }

        public async Task<bool> DeleteActionPlan(Guid actionPlanId)
        {
            if (await _repoActionPlans.GetItemAsync(actionPlanId) != null)
                return await _repoActionPlans.DeleteItemAsync(actionPlanId);
            return true;
        }
        #endregion
    }
}
