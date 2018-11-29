using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ResponseActionModel
    {
        public Guid ActionId { get; set; }
        public string ActionType { get; set; }
        public ActionStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Dictionary<string,string> Parameters { get; set; }
    }
}
