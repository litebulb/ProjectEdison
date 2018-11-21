using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ChatReportLogModel
    {
        public ChatUserModel From { get; set; }
        public string Message { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
        public string Id { get; set; }
        public bool IsBroadcast { get; set; }
        public Guid? ReportType { get; set; }
    }
}
