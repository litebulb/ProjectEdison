using Edison.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class EventClusterUIModel : IUIUpdateSignalR
    {
        public string UpdateType { get; set; }
        public EventClusterModel EventCluster { get; set; }
    }
}
