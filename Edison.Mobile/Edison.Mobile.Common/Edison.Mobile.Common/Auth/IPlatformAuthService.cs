using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Common.Auth
{
    public interface IPlatformAuthService
    {
        UIParent UiParent { get; set; }

        Task<AuthenticationResult> AcquireTokenAsync();
    }
}
