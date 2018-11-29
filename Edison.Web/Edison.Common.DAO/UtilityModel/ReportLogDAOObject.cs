using Edison.Core.Common;
using Newtonsoft.Json;
using System;

namespace Edison.Common.DAO
{
    public class ReportLogDAOObject
    {
        public ChatUserDAOObject From { get; set; }
        public string Message { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
        public string Id { get; set; }
        public bool IsBroadcast { get; set; }
        public Guid? ReportType { get; set; }
    }
}
