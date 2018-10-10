using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;

namespace Edison.Common.Chat.Models
{
    [Serializable]
    public class Party : IEquatable<Party>
    {
        public string ServiceUrl { get; set; }
        
        public string ChannelId { get; set; }
        
        public ChannelAccount ChannelAccount { get; set; }        

        public ConversationAccount ConversationAccount { get; set; }
        
        public DateTime ConnectionRequestTime { get; set; }
        
        public DateTime ConnectionEstablishedTime { get; set; }

        public string MessageRequestString { get; set; }

        public Party(string serviceUrl, string channelId,
            ChannelAccount channelAccount, ConversationAccount conversationAccount)
        {
            if (string.IsNullOrEmpty(serviceUrl))
            {
                throw new AccessViolationException(nameof(serviceUrl));
            }

            if (string.IsNullOrEmpty(channelId))
            {
                throw new ArgumentException(nameof(channelId));
            }

            if (conversationAccount == null || string.IsNullOrEmpty(conversationAccount.Id))
            {
                throw new ArgumentException(nameof(conversationAccount));
            }

            ServiceUrl = serviceUrl;
            ChannelId = channelId;
            ChannelAccount = channelAccount;
            ConversationAccount = conversationAccount;

            ResetConnectionRequestTime();
            ResetConnectionEstablishedTime();
        }

        
        public bool HasMatchingChannelInformation(Party other)
        {
            return (other != null
                && other.ChannelId.Equals(ChannelId)
                && other.ChannelAccount != null
                && ChannelAccount != null
                && other.ChannelAccount.Id.Equals(ChannelAccount.Id));
        }

        
        public bool IsPartOfSameConversation(Party other)
        {
            return (other != null
                //&& other.ServiceUrl.Equals(ServiceUrl) // Service URL cannot be guaranteed, by the bot framework, to stay the same!
                && other.ChannelId.Equals(ChannelId)
                && other.ConversationAccount.Id.Equals(ConversationAccount.Id));
        }

        public void ResetConnectionRequestTime()
        {
            ConnectionRequestTime = DateTime.MinValue;
        }

        public void ResetConnectionEstablishedTime()
        {
            ConnectionEstablishedTime = DateTime.MinValue;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Party FromJsonString(string jsonString)
        {
            return JsonConvert.DeserializeObject<Party>(jsonString);
        }

        public bool Equals(Party other)
        {
            return (IsPartOfSameConversation(other)
                && ((other.ChannelAccount == null && ChannelAccount == null)
                    || (other.ChannelAccount != null && ChannelAccount != null
                        && other.ChannelAccount.Id == ChannelAccount.Id)));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Party);
        }

        public override int GetHashCode()
        {
            string channelAccountId = (ChannelAccount != null) ? ChannelAccount.Id : "0";
            return new { ServiceUrl, ChannelId, channelAccountId, ConversationAccount.Id }.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{ServiceUrl}; {ChannelId}; "
                + ((ChannelAccount == null) ? "(no specific user); " : ($"{{{ChannelAccount.Id}; {ChannelAccount.Name}}}; "))
                + $"{{{ConversationAccount.Id}; {ConversationAccount.Name}}}]";
        }
    }
}