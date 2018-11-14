using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public class ActionEmailEvent : ActionBase, IActionEmailEvent
    {
        public ActionEmailEvent(ActionModel model, bool isCloseAction = false) : base(model, isCloseAction) { }
        public string Subject { get { return GetProperty<string>("subject"); } set { SetProperty("subject", value); } }
        public string ToLine { get { return GetProperty<string>("toline"); } set { SetProperty("toline", value); } }
        public string CCLine { get { return GetProperty<string>("ccline"); } set { SetProperty("ccline", value); } }
        public string Body { get { return GetProperty<string>("body"); } set { SetProperty("body", value); } }
    }
}
