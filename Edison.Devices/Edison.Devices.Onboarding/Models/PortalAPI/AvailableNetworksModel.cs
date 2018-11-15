using System.Collections.Generic;

namespace Edison.Devices.Onboarding.Models.PortalAPI
{
    public sealed class AvailableNetworksModel
    {
        public IEnumerable<AvailableNetwork> AvailableNetworks { get; set; }
    }
}
