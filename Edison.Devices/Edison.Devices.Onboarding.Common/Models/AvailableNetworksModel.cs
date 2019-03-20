using System.Collections.Generic;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class AvailableNetworksModel
    {
        public IEnumerable<AvailableNetwork> AvailableNetworks { get; set; }
    }
}
