using Edison.Devices.Onboarding.Common.Helpers;
using System;
using System.Text;
using Windows.Storage;

namespace Edison.Devices.Onboarding.Helpers
{
    public sealed class SecretManager
    {
        private const string CERTIFICATE_PASSKEY = "CertificatePasskey";
        private const string ENCRYPTION_KEY = "EncryptionKey";
        private const string PORTAL_PASSWORD = "PortalPassword";
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private static string _PortalPasswordBase64;

        public static string AccessPointSsid
        {
            get
            {
                if (_localSettings.Values.ContainsKey("AccessPointSsid"))
                    return _localSettings.Values["AccessPointSsid"].ToString();
                return SharedConstants.DEFAULT_AP_SSID;
            }
            set
            {
                _localSettings.Values["AccessPointSsid"] = value;
            }
        }

        public static string AccessPointPassword
        {
            get
            {
                if (_localSettings.Values.ContainsKey("AccessPointPassword"))
                    return _localSettings.Values["AccessPointPassword"].ToString();
                return SharedConstants.DEFAULT_AP_PASSWORD;
            }
            set
            {
                _localSettings.Values["AccessPointPassword"] = value;
            }
        }

        public static string EncryptionKey
        {
            get
            {
                if (_localSettings.Values.ContainsKey(ENCRYPTION_KEY))
                    return _localSettings.Values[ENCRYPTION_KEY].ToString();
                return SharedConstants.DEFAULT_ENCRYPTION_KEY;
            }
            set
            {
                _localSettings.Values[ENCRYPTION_KEY] = value;
            }
        }

        public static string CertificatePasskey
        {
            get
            {
                if (_localSettings.Values.ContainsKey(CERTIFICATE_PASSKEY))
                    return _localSettings.Values[CERTIFICATE_PASSKEY].ToString();
                return SharedConstants.DEFAULT_CERTIFICATE_PASSKEY;
            }
            set
            {
                _localSettings.Values[CERTIFICATE_PASSKEY] = value;
            }
        }

        public static string PortalPassword
        {
            get
            {
                if (_localSettings.Values.ContainsKey(PORTAL_PASSWORD))
                    return _localSettings.Values[PORTAL_PASSWORD].ToString();
                return SharedConstants.DEFAULT_PORTAL_PASSWORD;
            }
            set
            {
                _localSettings.Values[PORTAL_PASSWORD] = value;
                _PortalPasswordBase64 = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"Administrator:{value}"));
            }
        }

        public static string PortalPasswordBase64
        {
            get
            {
                return _PortalPasswordBase64;
            }
        }
    }
}
