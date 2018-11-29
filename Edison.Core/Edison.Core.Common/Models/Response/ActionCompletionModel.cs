using System;

namespace Edison.Core.Common.Models
{
    public class ActionCompletionModel
    {
        public Guid ResponseId { get; set; }
        public Guid ActionId { get; set; }
        public ActionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ErrorMessage { get; set; }
    }
}
