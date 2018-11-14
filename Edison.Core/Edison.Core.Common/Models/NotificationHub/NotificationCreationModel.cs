using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class NotificationCreationModel
    {
        public string Title { get; set; }
        public string User { get; set; }
        public Guid ResponseId { get; set; }
        public string NotificationText { get; set; }
        public List<string> Tags { get; set; }
        public int Status { get; set; }
    }
}
