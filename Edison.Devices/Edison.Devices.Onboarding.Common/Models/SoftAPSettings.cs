using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class SoftAPSettings
    {
        public bool SoftAPEnabled { get; set; }
        public string SoftApPassword { get; set; }
        public string SoftApSsid { get; set; }
    }
}
