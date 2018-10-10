using Edison.Core.Common.Models;
using System.Collections.Generic;

namespace Edison.EventProcessorService.Models
{
    public class DeviceTriggerIoTMessage
    {
        public Dictionary<string, object> Metadata { get; set; }
    }
}
