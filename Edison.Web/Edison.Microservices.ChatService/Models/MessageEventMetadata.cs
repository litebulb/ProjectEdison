using System;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Object that represents the metadata to be sent in the message event to the event processor
    /// </summary>
    [Serializable]
    public class MessageEventMetadata
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public Guid? ReportType { get; set; }
        public string Message { get; set; }
        public Guid ChatReportId { get; set; }
    }
}
