using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IResponseUIUpdated : IMessage
    {
        Guid ResponseId { get; set; }
    }
}
