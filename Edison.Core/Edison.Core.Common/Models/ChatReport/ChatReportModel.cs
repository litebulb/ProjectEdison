using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ChatReportModel
    {
        public Guid ReportId { get; set; }
        public string ChannelId { get; set; }
        public ChatUserModel User { get; set; }
        public List<ChatReportLogModel> ReportLogs { get; set; }
        public DateTime? EndDate { get; set; }
        public string ETag { get; set; }
    }
}
