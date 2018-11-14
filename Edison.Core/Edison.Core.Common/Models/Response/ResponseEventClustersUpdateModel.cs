using Edison.Core.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ResponseEventClustersUpdateModel
    {
        public Guid ResponseId { get; set; }
        public IEnumerable<Guid> EventClusterIds { get; set; }
    }
}
