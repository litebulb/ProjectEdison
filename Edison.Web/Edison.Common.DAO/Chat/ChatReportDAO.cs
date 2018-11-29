using Edison.Common.Interfaces;
using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    /// <summary>
    /// DAO - Keeps track of a conversation between one consumer and the administrators. 
    /// </summary>
    public class ChatReportDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ChannelId { get; set; }
        public ChatUserDAOObject User { get; set; }
        public List<ReportLogDAOObject> ReportLogs { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? EndDate { get; set; }
        public string ETag { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CreationDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime UpdateDate { get; set; }
    }
}
