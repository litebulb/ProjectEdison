using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Edison.Api
{
    [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb")]
    public class SignalRHub : Hub
    {
        public SignalRHub()
        {
        }
    }
}
