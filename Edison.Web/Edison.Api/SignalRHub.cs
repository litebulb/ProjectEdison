using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Edison.Api
{
    //[Authorize(AuthenticationSchemes = "Backend,B2C")]
    public class SignalRHub : Hub
    {
        public SignalRHub()
        {
        }
    }
}
