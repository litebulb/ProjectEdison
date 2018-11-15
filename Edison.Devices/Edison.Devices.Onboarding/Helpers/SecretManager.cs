using Edison.Devices.Onboarding.Common.Helpers;
using Windows.Storage;

namespace Edison.Devices.Onboarding.Helpers
{
    public sealed class SecretManager
    {
        private const string CERTIFICATE_PASSKEY = "CertificatePasskey";
        private const string SOCKET_PASSPHRASE = "SocketPassphrase";
        private const string PORTAL_PASSWORD = "PortalPassword";
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public static string SocketPassphrase
        {
            get
            {
                if (_localSettings.Values.ContainsKey(SOCKET_PASSPHRASE))
                    return _localSettings.Values[SOCKET_PASSPHRASE].ToString();
                return SharedConstants.DEFAULT_SOCKET_PASSPHRASE;
            }
            set
            {
                _localSettings.Values[SOCKET_PASSPHRASE] = value;
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
            }
        }
    }
}
