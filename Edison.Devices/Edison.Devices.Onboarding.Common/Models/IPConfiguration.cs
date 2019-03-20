using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models
{
    public class IPAdapters
    {
        public IEnumerable<IPConfiguration> Adapters { get; set; }
    }

    public class IPConfiguration
    {
        public string DDNSEnabled { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public IEnumerable<IPAddress> IpAddresses { get; set; }
    }

    public class IPAddress
    {
        public string IpAddress { get; set; }
        public string Mask { get; set; }
    }
}
