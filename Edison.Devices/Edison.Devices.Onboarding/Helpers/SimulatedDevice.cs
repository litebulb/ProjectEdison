using Microsoft.Devices.Tpm;
using System;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

namespace Edison.Devices.Onboarding.Helpers
{
    /// <summary>
    /// Using a simulated TPM. For a real TPM, the certificate private key should be stored in HSM/TPM.
    /// </summary>
    internal class SimulatedDevice
    {
        private const string PRIVATE_KEY = "iotdevice";
        private static readonly Guid _temporaryDeviceId = Guid.NewGuid();
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public static Guid DeviceId
        {
            get
            {
                TpmDevice tpm = new TpmDevice(0);
                string deviceId = tpm.GetDeviceId();
                if(!Guid.TryParse(deviceId, out Guid parsed))
                    return _temporaryDeviceId;
                return parsed;
            }
        }

        public static bool IsProvisioned
        {
            get
            {
                if (_localSettings.Values.ContainsKey("IsProvisioned"))
                    return bool.Parse(_localSettings.Values["IsProvisioned"].ToString());
                return false;
            }
            set
            {
                if (value == true)
                    _localSettings.Values["IsProvisioned"] = "true";
                else
                    _localSettings.Values["IsProvisioned"] = "false";
            }
        }

        public static string IoTHubHostname
        {
            get
            {
                TpmDevice tpm = new TpmDevice(0);
                return tpm.GetHostName();
            }
        }

        public static void ClearRSAKey()
        {
            if (CngKey.Exists(PRIVATE_KEY))
            {
                using (CngKey algorithmKey = CngKey.Open(PRIVATE_KEY))
                {
                    algorithmKey.Delete();
                }
            };
        }

        public static RSA GenerateRSAKey()
        {
            ClearRSAKey();

            CngKeyCreationParameters creationParameters = new CngKeyCreationParameters()
            {
                ExportPolicy = CngExportPolicies.AllowExport | CngExportPolicies.AllowPlaintextExport
            };
            creationParameters.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(1024), CngPropertyOptions.Persist));

            CngKey algorithmKey = CngKey.Create(CngAlgorithm.Rsa, PRIVATE_KEY, creationParameters);
            
            return new RSACng(algorithmKey);
        }

        public static RSA GetPrivateKey(bool generateIfNull = false)
        {
            if (CngKey.Exists(PRIVATE_KEY))
            {
                CngKey algorithmKey = CngKey.Open(PRIVATE_KEY);
                return new RSACng(algorithmKey);
            }
            if (generateIfNull)
                return GenerateRSAKey();
            throw new Exception("No private key generated");
        }

        public static void ProvisionDevice(string hostname, string deviceId)
        {
            TpmDevice tpm = new TpmDevice(0);
            if (Guid.TryParse(tpm.GetDeviceId(), out Guid parsed))
            {
                try
                {
                    tpm.Destroy();
                }
                catch
                {
                    DebugHelper.LogWarning($"Error while resetting TPM.");
                }
            }

            //The real private key can be stored in HSM/TPM
            //The Microsoft.Devices.TPM is used solemnly for TPM enrollement, and not certificate, here we're just using it to persist hostname/deviceId
            tpm.Provision(Convert.ToBase64String(Encoding.ASCII.GetBytes("REALKEYINTPM")), hostname, deviceId);

            ClearRSAKey();
        }
    }
}
