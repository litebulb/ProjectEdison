namespace Edison.Core.Config
{
    public class RestServiceOptions
    {
        public string RestServiceUrl { get; set; }
        public string SecretToken { get; set; }
        public AzureAdOptions AzureAd { get; set; }
    }
}
