using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceiveResponseUpdated : IMessage
    {
        ResponseModel ResponseModel { get; set; }
    }
}
