using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ReportLogCreationModel
    {
        public ChatUserModel User { get; set; }
        public string ChannelId { get; set; }
        public ReportLogModel Message { get; set; }
    }
}
