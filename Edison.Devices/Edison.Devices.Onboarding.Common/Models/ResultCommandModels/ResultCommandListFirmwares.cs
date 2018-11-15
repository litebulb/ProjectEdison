using System.Collections.Generic;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class ResultCommandListFirmwares : ResultCommand
    {
        public IEnumerable<string> Firmwares { get; set; }
    }
}
