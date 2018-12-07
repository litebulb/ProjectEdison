using System;
namespace Edison.Mobile.Common.Auth
{
    public interface IAppAuthService
    {
        UserInfo GetUserInfo(string idToken);
    }
}
