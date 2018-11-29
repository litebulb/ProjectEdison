using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventModel
    {
        public DateTime Date { get; set; }
        [JsonConverter(typeof(DynamicDictionaryCamelCaseConverter))]
        public Dictionary<string, object> Metadata { get; set; }
    }
}
