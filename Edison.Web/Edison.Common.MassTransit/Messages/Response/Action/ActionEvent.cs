using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ActionEvent : IActionEvent
    {
        public ActionEvent(bool _isCloseAction = false)
        {
            IsCloseAction = _isCloseAction;
        }
        public Guid ResponseId { get; set; }
        public ActionModel Action { get; set; }
        public bool IsCloseAction { get; set; }
        public Geolocation Geolocation { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
    }
}
