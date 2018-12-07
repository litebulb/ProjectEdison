using System.IdentityModel.Tokens.Jwt;
using Edison.Mobile.Common.Auth;

namespace Edison.Mobile.User.Client.Core.Auth
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
                    case "given_name":
                        userInfo.GivenName = claim.Value;
                        break;
                    case "family_name":
                        userInfo.FamilyName = claim.Value;
                        break;
                    case "emails":
                        userInfo.Email = claim.Value;
                        break;
                }
            }

            return userInfo;
        }
    }
}
