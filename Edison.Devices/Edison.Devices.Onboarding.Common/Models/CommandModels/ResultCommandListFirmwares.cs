using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models.CommandModels
{
    public sealed class ResultCommandListFirmwares
    {
        public IEnumerable<string> Firmwares { get; set; }
    }
}
