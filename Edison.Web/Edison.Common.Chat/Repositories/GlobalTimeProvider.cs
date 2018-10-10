using System;

namespace Edison.Common.Chat.Repositories
{
    public class GlobalTimeProvider
    {
        public virtual DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }
    }
}
