using System;
using System.Collections.Generic;

namespace Edison.Mobile.Common.Auth
{
    public static class AuthConfig
    {
        public static string UserRedirectUri { get; } = "com.onmicrosoft.edisondevb2c.user://redirect/";
        public static string AdminRedirectUri { get; } = "com.onmicrosoft.edisondevb2c.admin://redirect/";

        //public static string ReadScope { get; } = "https://edisondevb2c.onmicrosoft.com/web/write";
        //public static string WriteScope { get; } = "https://edisondevb2c.onmicrosoft.com/web/read";

        public static IEnumerable<string> Scopes { get; } = new string[] { "email" };

        public static string Tenant { get; } = "edisondevb2c.onmicrosoft.com";
        public static string Policy { get; } = "b2c_1_edision_signinandsignup";
        public static string BaseAuthority { get; } = $"https://login.microsoftonline.com/tfp/{Tenant}/";

        public static string Authority { get; } = $"{BaseAuthority}{Policy}";
    }
}
