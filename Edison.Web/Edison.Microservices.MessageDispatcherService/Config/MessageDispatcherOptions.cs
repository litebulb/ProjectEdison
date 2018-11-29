using System.Collections.Generic;

namespace Edison.MessageDispatcherService.Config
{
    public class MessageDispatcherOptions
    {
        public bool DispatcherEnabled { get; set; }
        public string DefaultMessageType { get; set; }
        public string DefaultEventType { get; set; }
        public List<string> PropertyDeviceId { get; set; }
        public List<string> PropertyMessageType { get; set; }
        public List<string> PropertyEventType { get; set; }
    }
}
