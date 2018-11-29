using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Edison.Core.Common;

namespace Edison.Api
{
    /// <summary>
    /// SignalR class
    /// </summary>
    [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C)]
    public class SignalRHub : Hub
    {
        public SignalRHub()
        {
        }
    }
}
