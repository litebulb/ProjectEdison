using Edison.Devices.Onboarding.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Devices.Onboarding.Client.Models
{
    public class DeviceCertificateRequestModel
    {
        public string DeviceType { get; set; }
        public string Csr { get; set; }
        //public string DeviceId { get; set; }
    }
}
