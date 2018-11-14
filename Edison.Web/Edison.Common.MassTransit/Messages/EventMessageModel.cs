using Edison.Core.Common.Models;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class EventMessageModel
    {
        public Dictionary<string, object> Metadata { get; set; }
        //public EventSourceModel SourceData { get; set; }
    }
}
