using System.Collections.Generic;

namespace Edison.Devices.Onboarding.Models.PortalAPI
{
    public sealed class AvailableNetwork
    {
        public bool AlreadyConnected { get; set; }
        public string AuthenticationAlgorithm { get; set; }
        public string CipherAlgorithm { get; set; }
        public int Connectable { get; set; }
        public string InfrastructureType { get; set; }
        public bool ProfileAvailable { get; set; }
        public string ProfileName { get; set; }
        public string SSID { get; set; }
        public bool SecurityEnabled { get; set; }
        public int SignalQuality { get; set; }
        public IEnumerable<string> PhysicalTypes { get; set; }
    }
}
