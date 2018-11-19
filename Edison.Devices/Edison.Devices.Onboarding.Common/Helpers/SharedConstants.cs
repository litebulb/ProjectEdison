using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Helpers
{
    public class SharedConstants
    {
        public const string DEFAULT_CERTIFICATE_PASSKEY = "Edison1234";
        public const string DEFAULT_ENCRYPTION_KEY = "c7c0dd2d09a4efc7d2f39f53fd9b207f5b7aafa272639c4b8771f9ba60b14400";
        public const string DEFAULT_PORTAL_PASSWORD = "Edison1234";
        public const string DEFAULT_AP_SSID = "EDISON";
        public const string DEFAULT_AP_PASSWORD = "Edison1234";

        public const int DEVICE_API_PORT = 9000;
        public const string DEVICE_HOST_API_IP = "192.168.137.1";
        public const string DEVICE_DEBUG_API_IP = "192.168.137.1";//"192.168.0.24";//"192.168.137.1";
    }
}
