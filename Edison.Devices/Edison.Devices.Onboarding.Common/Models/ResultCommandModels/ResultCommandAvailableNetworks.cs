using System.Collections.Generic;

namespace Edison.Devices.Onboarding.Common.Models
{
    public class ResultCommandAvailableNetworks : ResultCommand
    {
        public IEnumerable<AvailableNetwork> Networks { get; set; }
    }
}
