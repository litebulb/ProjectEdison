using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceiveResponseLocated : IMessage
    {
        ResponseModel ResponseModel { get; set; }
    }
}
