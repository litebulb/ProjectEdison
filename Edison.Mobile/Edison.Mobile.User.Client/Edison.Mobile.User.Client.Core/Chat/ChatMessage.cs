using Edison.Core.Common.Models;

namespace Edison.Mobile.User.Client.Core.Chat
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public ChatUserModel UserModel { get; set; }
        public ActionPlanListModel ActionPlan { get; set; }
        public bool IsNewActionPlan { get; set; }
    }
}
