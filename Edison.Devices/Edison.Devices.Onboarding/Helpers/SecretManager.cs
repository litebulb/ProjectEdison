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
        private const string IS_ENCRYPTION_ENABLED = "IsEncryptionEnabled";
        private const string PORTAL_PASSWORD = "PortalPassword";
        private const string AP_SSID = "AccessPointSsid";
        private const string AP_PASSWORD = "AccessPointPassword";
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private static string _PortalPasswordBase64;

        public static string AccessPointSsid
        {
            get
            {
                if (_localSettings.Values.ContainsKey(AP_SSID))
                    return _localSettings.Values[AP_SSID].ToString();
                return SharedConstants.DEFAULT_AP_SSID;
            }
            set
            {
                _localSettings.Values[AP_SSID] = value;
            }
        }

        public static string AccessPointPassword
        {
            get
            {
                if (_localSettings.Values.ContainsKey(AP_PASSWORD))
                    return _localSettings.Values[AP_PASSWORD].ToString();
                return SharedConstants.DEFAULT_AP_PASSWORD;
            }
            set
            {
                _localSettings.Values[AP_PASSWORD] = value;
            }
        }

        public static bool IsEncryptionEnabled
        {
            get
            {
                if (_localSettings.Values.ContainsKey(IS_ENCRYPTION_ENABLED))
                    return bool.Parse(_localSettings.Values[IS_ENCRYPTION_ENABLED].ToString());
                return false;
            }
            set
            {
                if (value == true)
                    _localSettings.Values[IS_ENCRYPTION_ENABLED] = "true";
                else
                    _localSettings.Values[IS_ENCRYPTION_ENABLED] = "false";
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
