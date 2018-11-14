using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Edison.Core.Common
{
    public class UserHelper
    {
        public static string GetBestClaimValue(string key, List<Claim> claimsList, List<string> claims, bool mandatory = false)
        {
            foreach (string claimTest in claims)
            {
                string value = claimsList.Find(p => p.Type == claimTest)?.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            if (mandatory)
                throw new Exception($"Claims not found for '{key}'!");
            return string.Empty;
        }

        public static string GetBestClaimValue(List<Claim> claimsList, List<string> claims, bool mandatory = false)
        {
            foreach (string claimTest in claims)
            {
                string value = claimsList.Find(p => p.Type == claimTest)?.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            if (mandatory)
                throw new Exception($"Claims not found!");
            return string.Empty;
        }
    }
}
