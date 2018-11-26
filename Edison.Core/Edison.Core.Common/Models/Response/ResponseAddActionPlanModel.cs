using Edison.Core.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ResponseChangeActionPlanModel
    {
        public Guid ResponseId { get; set; }
        public List<ActionChangedModel> Actions { get; set; }
    }

    public class ActionChangedModel
    {
        public ActionModel Action { get; set; }
        public bool IsCloseAction { get; set; }
        public string ActionChangedString { get; set; }
    }
}
