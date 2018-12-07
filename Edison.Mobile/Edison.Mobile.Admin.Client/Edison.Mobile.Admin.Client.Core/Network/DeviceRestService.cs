using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;

namespace Edison.Mobile.Admin.Client.Core.Network
{
    public class DeviceRestService : BaseRestService
    {
        public DeviceRestService(AuthService authService, ILogger logger, string baseUrl)
            : base(authService, logger, baseUrl)
        {
        }

        public async Task<DeviceModel> GetDevices()
        {
            throw new NotImplementedException();
        }
    }
}
