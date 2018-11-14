using Edison.Core.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class DeviceUIModel : IUIUpdateSignalR
    {
        public string UpdateType { get; set; }
        public Guid DeviceId { get; set; }
        public DeviceModel Device { get; set; }
    }
}
