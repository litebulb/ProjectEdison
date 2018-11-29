using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ResponseModel
    {
        public Guid ResponseId { get; set; }
        public Guid ResponderUserId { get; set; }
        public int ResponseState { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public List<string> SafeUsers { get; set; }
        public ResponseActionPlanModel ActionPlan { get; set; }
        public Guid? PrimaryEventClusterId { get; set; }
        public Geolocation Geolocation { get; set; }
        public IEnumerable<Guid> EventClusterIds { get; set; }
    }
}
