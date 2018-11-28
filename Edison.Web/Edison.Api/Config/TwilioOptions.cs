namespace Edison.Api.Config
{
    public class TwilioOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string Region { get; set; }
        public string HttpClient { get; set; }
        public string PhoneNumber { get; set; }
        //For Emergency Number Setup
        public string EmergencyPhoneNumber { get; set; }
        //public string EmergencyStreet { get; set; }
        //public string EmergencyZipCode { get; set; }
        //public string EmergencyRegionStateCode { get; set; }
        //public string EmergencyIsoCountry { get; set; }
        //public string EmergencyCompanyName { get; set; }
    }
}
