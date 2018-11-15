using System.Collections.Generic;

namespace Edison.Devices.Onboarding.Models.PortalAPI
{
    internal class PackageSetModel
    {
        public bool HolographicAvailable { get; set; }
        public List<PackageModel> InstalledPackages { get; set; }
    }
}
