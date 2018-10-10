namespace Edison.Common.Chat.Config
{
    public class BotOptions
    {
        public string MicrosoftAppId { get; set; }

        public string MicrosoftAppPassword { get; set; }

        public string AzureStorageConnectionString { get; set; }

        public string KeyRejectConnectionRequestIfNoAggregationChannel { get; set; }

        public bool RejectConnectionRequestIfNoAggregationChannel
        {
            get
            {
                string settingValueAsString = this.KeyRejectConnectionRequestIfNoAggregationChannel;

                if (!string.IsNullOrEmpty(settingValueAsString)
                    && settingValueAsString.ToLower().Trim().Equals("true"))
                {
                    return true;
                }

                return false;
            }
        }      
    }
}