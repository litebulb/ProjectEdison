using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Schema;
using System.Security.Claims;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.ChatService.Config;
using Edison.ChatService.Helpers;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Class that adds some helper methods to generate a ChatUserModel
    /// </summary>
    public class ChatUserContext : ChatUserModel
    {
        public static ChatUserContext FromClaims(IEnumerable<Claim> claims, BotOptions config)
        {
            List<Claim> claimsList = claims.ToList();

            //Get id
            string id = UserHelper.GetBestClaimValue("id", claimsList, config.ClaimsId, true);

            //Get name
            string firstName = UserHelper.GetBestClaimValue("firstName", claimsList, config.ClaimsFirstName, true);
            string lastName = UserHelper.GetBestClaimValue(claimsList, config.ClaimsLastName);

            //Get role
            string role = UserHelper.GetBestClaimValue(claimsList, config.ClaimsRole);
            ChatUserRole roleEnum = ChatUserRole.Consumer;
            if (!string.IsNullOrEmpty(role) && config.ClaimsRoleAdminValues.Contains(role))
                roleEnum = ChatUserRole.Admin;
            UserRoleCache.UserRoles[$"dl_{id}"] = roleEnum;

            var userContext = new ChatUserContext()
            {
                Id = $"dl_{id}",
                Name = $"{firstName} {lastName}",
                Role = roleEnum
            };

            return userContext;
        }

        public static ChatUserContext FromConversation(ConversationReference conversation)
        {
            return new ChatUserContext()
            {
                Id = conversation.User?.Id,
                Name = conversation.User?.Name,
                Role = GetUserRoleFromString(conversation.User?.Role)
            };
        }

        private static ChatUserRole GetUserRoleFromString(string roleStr)
        {
            Enum.TryParse(typeof(ChatUserRole), roleStr, out object role);
            if (role == null)
                role = ChatUserRole.Consumer;
            return (ChatUserRole)role;
        }
    }
}
