using Edison.Core;
using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class EventDAOObject
    {
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
