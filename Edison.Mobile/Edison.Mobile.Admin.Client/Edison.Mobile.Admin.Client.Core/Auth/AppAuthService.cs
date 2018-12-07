using System;
using System.IdentityModel.Tokens.Jwt;
using Edison.Mobile.Common.Auth;

namespace Edison.Mobile.Admin.Client.Core.Auth
{
    public class AppAuthService : IAppAuthService
    {
        public UserInfo GetUserInfo(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);
            var userInfo = new UserInfo();

            foreach (var claim in token.Claims)
            {
                switch (claim.Type)
                {
                    case "name":
                        var fullName = claim.Value;
                        var names = fullName.Split(',');
                        if (names.Length > 1)
                        {
                            userInfo.FamilyName = names[0].Trim();
                            userInfo.GivenName = names[1].Trim();
                        }
                        break;
                    case "email":
                        userInfo.Email = claim.Value;
                        break;
                }
            }

            return userInfo;
        }
    }
}
