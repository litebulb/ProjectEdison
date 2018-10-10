using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Interfaces
{
    public interface IActionPlanModel
    {
        Guid ActionPlanId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime CreationDate { get; set; }
        DateTime UpdateDate { get; set; }
        bool IsActive { get; set; }
        IEnumerable<ActionModel> OpenActions { get; set; }
        IEnumerable<ActionModel> CloseActions { get; set; }
    }
}
