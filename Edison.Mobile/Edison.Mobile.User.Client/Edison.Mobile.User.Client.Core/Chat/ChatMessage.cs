using Edison.Core.Common.Models;

namespace Edison.Mobile.User.Client.Core.Chat
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public ChatUserModel UserModel { get; set; }
    }
}
