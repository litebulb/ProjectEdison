namespace Edison.Core.Common.Models
{
    public class ChatReportLogCreationModel
    {
        public ChatUserModel User { get; set; }
        public string ChannelId { get; set; }
        public ChatReportLogModel Message { get; set; }
    }
}
