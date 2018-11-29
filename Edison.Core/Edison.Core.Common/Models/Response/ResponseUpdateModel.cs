using System;

namespace Edison.Core.Common.Models
{
    public class ResponseUpdateModel
    {
        public Guid ResponseId { get; set; }
        public ResponseActionPlanModel ActionPlan { get; set; }
        public Geolocation Geolocation { get; set; }
    }
}
