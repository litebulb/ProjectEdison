using System;

namespace Edison.Mobile.Common.Auth
{
    public class AuthChangedEventArgs : EventArgs
    {
        public bool IsLoggedIn { get; set; }
        public bool WasTokenAcquiredSilently { get; set; }
    }
}