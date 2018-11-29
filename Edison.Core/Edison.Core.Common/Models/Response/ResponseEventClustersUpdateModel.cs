using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ResponseEventClustersUpdateModel
    {
        public Guid ResponseId { get; set; }
        public IEnumerable<Guid> EventClusterIds { get; set; }
    }
}
