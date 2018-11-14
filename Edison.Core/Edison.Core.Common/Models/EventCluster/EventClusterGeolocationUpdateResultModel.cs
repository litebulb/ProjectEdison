using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventClusterGeolocationUpdateResultModel
    {
        public bool Success { get; set; }
        public EventClusterModel EventCluster { get; set; }     
    }
}
