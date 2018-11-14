using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models
{
    public class DeviceCertificateModel
    {
        public string Certificate { get; set; }
        public string DpsInstance { get; set; }
        public string DpsIdScope { get; set; }
        public string DeviceType { get; set; }
    }
}
