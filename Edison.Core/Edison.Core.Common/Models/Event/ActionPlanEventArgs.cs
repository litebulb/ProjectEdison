using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ActionPlanEventArgs : EventArgs
    {
        public ActionPlanListModel SelectedActionPlan { get; set; }
    }
}
