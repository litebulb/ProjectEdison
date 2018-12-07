using System;
using System.Collections.Generic;

namespace Edison.Mobile.Common.Auth
{
    public static class AuthConfig
    {
        static readonly string adminTenant = "edisonbluemetal.onmicrosoft.com";

        static readonly string userTenant = "edisondevb2c.onmicrosoft.com";
        static readonly string userPolicy = "b2c_1_edision_signinandsignup";
        static readonly string userBaseAuthority = $"https://login.microsoftonline.com/tfp/{userTenant}/";

        public static string UserRedirectUri { get; } = "com.onmicrosoft.edisondevb2c.user://redirect/";
        public static string AdminRedirectUri { get; } = "com.onmicrosoft.edisonadmin://redirect/";

        public static IEnumerable<string> Scopes { get; } = new string[] { "email" };

        public static string UserAuthority { get; } = $"{userBaseAuthority}{userPolicy}";
        public static string AdminAuthority = $"https://login.microsoftonline.com/{adminTenant}";
    }
}
