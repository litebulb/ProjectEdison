using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class ResultCommandNetworkStatus : ResultCommand
    {
        public string Status { get; set; }
    }
}
