using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class NotificationModel
    {
        public string Title { get; set; }
        public string User { get; set; }
        public string NotificationText { get; set; }
        public List<string> Tags { get; set; }
        public int Status { get; set; }
        public Guid NotificationId { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CreationDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime UpdateDate { get; set; }
    }
}
