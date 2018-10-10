using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public class ActionRapidSOSEvent : ActionBase, IActionRapidSOSEvent
    {
        public ActionRapidSOSEvent(ActionModel model) : base(model) { }
        public string ServiceType { get { return GetProperty<string>("servicetype"); } set { SetProperty("servicetype", value); } }
        public string Message { get { return GetProperty<string>("message"); } set { SetProperty("message", value); } }
    }
}
