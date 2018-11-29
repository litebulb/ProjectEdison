using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public class ActionNotificationEvent : ActionBase, IActionNotificationEvent
    {
        public ActionNotificationEvent(ResponseActionModel model) : base(model) { }
        public string User { get { return "notimplemented"; } set { SetProperty("user", value); } }
        public string Message { get { return GetProperty<string>("message"); } set { SetProperty("message", value); } }
        public bool IsSilent { get { return GetProperty<bool>("issilent"); } set { SetProperty("issilent", value.ToString()); } }
    }
}
