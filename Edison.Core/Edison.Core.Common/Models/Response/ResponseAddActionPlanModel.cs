using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ResponseChangeActionPlanModel
    {
        public Guid ResponseId { get; set; }
        public List<ActionChangedModel> Actions { get; set; }
    }

    public class ActionChangedModel
    {
        public ResponseActionModel Action { get; set; }
        public bool IsCloseAction { get; set; }
        public string ActionChangedString { get; set; }
    }
}
