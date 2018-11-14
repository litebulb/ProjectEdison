using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseActionsUpdated : IEventSagaReceiveResponseActionsUpdated
    {
        public Guid ResponseId { get; set; }
        public IEnumerable<ActionModel> Actions { get; set; }
        public Geolocation Geolocation { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
    }
}
