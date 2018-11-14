using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Models.PortalAPI
{
    internal class PackageSetModel
    {
        public bool HolographicAvailable { get; set; }
        public List<PackageModel> InstalledPackages { get; set; }
    }
}
