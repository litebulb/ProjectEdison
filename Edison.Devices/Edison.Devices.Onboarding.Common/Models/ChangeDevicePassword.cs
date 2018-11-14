using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models
{
    public sealed class ChangeDevicePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
