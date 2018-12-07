using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public class ActionTwilioEvent : ActionBase, IActionTwilioEvent
    {
        public ActionTwilioEvent(ResponseActionModel model) : base(model) { }
        public string Message { get { return GetProperty<string>("message"); } set { SetProperty("message", value); } }
    }
}
