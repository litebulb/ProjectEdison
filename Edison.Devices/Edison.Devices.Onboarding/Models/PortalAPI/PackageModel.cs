namespace Edison.Devices.Onboarding.Models.PortalAPI
{
    internal class PackageModel
    {
        public int AppListEntry { get; set; }
        public bool CanUninstall { get; set; }
        public string Name { get; set; }
        public string PackageDisplayName { get; set; }
        public string PackageFamilyName { get; set; }
        public string PackageFullName { get; set; }
        public int PackageOrigin { get; set; }
        public string PackageRelativeId { get; set; }
        public string Publisher { get; set; }
    }
}
