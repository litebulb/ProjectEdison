using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ReportLogCloseModel
    {
        public string UserId { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime EndDate { get; set; }
    }
}
