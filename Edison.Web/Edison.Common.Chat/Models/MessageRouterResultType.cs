
namespace Edison.Common.Chat.Models
{
    public enum MessageRouterResultType
    {
        NoActionTaken, // No action taken - The result handler should ignore results with this type
        OK, // Action taken, but the result handler should ignore results with this type
        ConnectionRequested,
        ConnectionAlreadyRequested,
        ConnectionRejected,
        Connected,
        Disconnected,
        NoAgentsAvailable,
        NoAggregationChannel,
        FailedToForwardMessage,
        Error // Generic error including e.g. null arguments
    }

}
