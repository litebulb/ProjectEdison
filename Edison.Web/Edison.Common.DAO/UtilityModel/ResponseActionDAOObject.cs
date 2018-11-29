using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Edison.Core.Common;

namespace Edison.Common.DAO
{
    public class ResponseActionDAOObject
    {
        public Guid ActionId { get; set; }
        public string ActionType { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? StartDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? EndDate { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
    }
}
