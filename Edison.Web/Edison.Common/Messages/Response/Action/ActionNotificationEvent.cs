using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public class ActionNotificationEvent : ActionBase, IActionNotificationEvent
    {
        public ActionNotificationEvent(ActionModel model) : base(model) { }
        public string PhoneNumber { get { return GetProperty<string>("phonenumber"); } set { SetProperty("phonenumber", value); } }
        public string Message { get { return GetProperty<string>("message"); } set { SetProperty("message", value); } }
        public bool IsSilent { get { return GetProperty<bool>("issilent"); } set { SetProperty("issilent", value.ToString()); } }
    }
}
