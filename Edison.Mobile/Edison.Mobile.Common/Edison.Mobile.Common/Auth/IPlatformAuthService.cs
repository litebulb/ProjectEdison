using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Common.Auth
{
    public interface IPlatformAuthService
    {
        Task<AuthenticationResult> AcquireTokenAsync();
    }
}
