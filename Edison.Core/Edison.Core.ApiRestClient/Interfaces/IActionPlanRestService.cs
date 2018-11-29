using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IActionPlanRestService
    {
        Task<ActionPlanModel> GetActionPlan(Guid actionPlanId);
        Task<IEnumerable<ActionPlanListModel>> GetActionPlans();
        Task<ActionPlanModel> UpdateActionPlan(ActionPlanUpdateModel actionPlanObj);
        Task<ActionPlanModel> CreateActionPlan(ActionPlanCreationModel actionPlanObj);
        Task<bool> DeleteActionPlan(Guid actionPlanId);
    }
}
