using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventClusterUpdateModel
    {
        public Guid EventClusterId { get; set; }
        public DateTime Date { get; set; }
        public IDictionary<string, object> Metadata { get; set; }
    }
}
