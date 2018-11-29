using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ActionEvent : IActionEvent
    {
        public Guid ActionCorrelationId { get; set; }
        public Guid ResponseId { get; set; }
        public Guid ActionId { get; set; }
        public ResponseActionModel Action { get; set; }
        public bool IsCloseAction { get; set; }
        public Geolocation Geolocation { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
    }
}
