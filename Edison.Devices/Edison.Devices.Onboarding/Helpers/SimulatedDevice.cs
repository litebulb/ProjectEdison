using Microsoft.Devices.Tpm;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Edison.Devices.Onboarding.Helpers
{
    /// <summary>
    /// Using a simulated TPM. For a real TPM, the certificate private key should be stored in HSM/TPM.
    /// </summary>
    internal class SimulatedDevice
    {
        private const string PRIVATE_KEY = "iothubdevice";
        private static readonly string _TemporaryDeviceId = Guid.NewGuid().ToString();

        public static string DeviceId
        {
            get
            {
                TpmDevice tpm = new TpmDevice(0);
                string deviceId = tpm.GetDeviceId();
                if(!Guid.TryParse(deviceId, out Guid parsed))
                    return _TemporaryDeviceId;
                return deviceId;
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

        public static RSA GetPrivateKey()
        {
            if (CngKey.Exists(PRIVATE_KEY))
            {
                CngKey algorithmKey = CngKey.Open(PRIVATE_KEY);
                return new RSACng(algorithmKey);
            }
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
