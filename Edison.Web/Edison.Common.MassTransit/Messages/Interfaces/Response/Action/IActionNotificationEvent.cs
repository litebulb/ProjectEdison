namespace Edison.Common.Messages.Interfaces
{
    public interface IActionNotificationEvent : IMessage, IActionBaseEvent
    {
        string User { get; set; }
        string Message { get; set; }
        bool IsSilent { get; set; }
    }
}
