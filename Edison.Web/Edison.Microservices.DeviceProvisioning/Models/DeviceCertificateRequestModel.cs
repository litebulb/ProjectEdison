using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.DeviceProvisioning.Models
{
    public class DeviceCertificateRequestModel
    {
        //public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Csr { get; set; }
    }
}
