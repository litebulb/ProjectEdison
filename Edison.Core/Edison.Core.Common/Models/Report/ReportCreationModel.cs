using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ReportCreationModel
    {
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? MinimumDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? MaximumDate { get; set; }
        public ReportCreationType Type { get; set; }
    }
}
