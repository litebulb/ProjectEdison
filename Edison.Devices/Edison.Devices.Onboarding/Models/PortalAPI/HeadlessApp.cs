using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Models.PortalAPI
{
    internal class HeadlessApp
    {
        public bool IsStartup { get; set; }
        public string PackageFullName { get; set; }
    }
}
