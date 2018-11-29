using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Edison.Api
{
    /// <summary>
    /// SignalR class
    /// </summary>
    [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb")]
    public class SignalRHub : Hub
    {
        public SignalRHub()
        {
        }
    }
}
