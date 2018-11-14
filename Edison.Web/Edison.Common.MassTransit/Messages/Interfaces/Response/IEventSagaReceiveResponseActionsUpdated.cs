using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceiveResponseActionsUpdated : IMessage
    {
        Guid ResponseId { get; set; }
        IEnumerable<ActionModel> Actions { get; set; }
        Geolocation Geolocation { get; set; }
        double PrimaryRadius { get; set; }
        double SecondaryRadius { get; set; }
    }
}
