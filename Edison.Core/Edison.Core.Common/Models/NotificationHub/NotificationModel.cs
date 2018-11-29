using System;
using System.Collections.Generic;

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
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
