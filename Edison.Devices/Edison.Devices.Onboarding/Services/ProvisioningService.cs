using Edison.Devices.Onboarding.Common.Models.CommandModels;
using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Microsoft.Azure.Devices.Client;
using Edison.Devices.Onboarding.Common.Helpers;
using System.Collections.Generic;

namespace Edison.Devices.Onboarding.Services
{
    internal class ProvisioningService
    {
        public ProvisioningService()
        {
        }

        public ResultCommandGetDeviceId GetDeviceId()
        {
            return new ResultCommandGetDeviceId()
            {
                DeviceId = SimulatedDevice.DeviceId
            };
        }

        public ResultCommandGenerateCSR GenerateCSR()
        {
            //Generate Certificate with RSA Private key
            byte[] csr = null;
            using (RSA key = SimulatedDevice.GenerateRSAKey())
            {
                CertificateRequest certRequest = new CertificateRequest($"CN={SimulatedDevice.DeviceId}",
                    key,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);
                csr = certRequest.CreateSigningRequest();
            }

            return new ResultCommandGenerateCSR()
            {
                Csr = csr != null ? Convert.ToBase64String(csr) : null
            };
        }

        public async Task<ResultCommand> ProvisionDevice(DeviceCertificateModel certificateInfo, string password)
        {
            try
            {
                //Load certificate chain
                var (deviceCertificate, collectionCertificates) = 
                    LoadCertificateFromPfx(Convert.FromBase64String(certificateInfo.Certificate), password);

                //Save certificate in store
                if(!await SaveCertificateInStore(deviceCertificate))
                    return ResultCommand.CreateFailedCommand($"Error while saving User Certificate in Store.");

                using (var securityProvider = new SecurityProviderX509Certificate(deviceCertificate, collectionCertificates))
                {
                    using (var transport = new ProvisioningTransportHandlerHttp())
                    {
                        ProvisioningDeviceClient provClient = 
                            ProvisioningDeviceClient.Create(certificateInfo.DpsInstance, certificateInfo.DpsIdScope, securityProvider, transport);
                        DeviceRegistrationResult result = await provClient.RegisterAsync();
                        if (result.Status != ProvisioningRegistrationStatusType.Assigned)
                        {
                            DebugHelper.LogError($"ProvisioningClient AssignedHub: {result.AssignedHub}; DeviceID: {result.DeviceId}");
                            return ResultCommand.CreateFailedCommand($"Error during registration: {result.Status}, {result.ErrorMessage}");
                        }

                        //Test the connection
                        if(!await TestDeviceConnection(result.DeviceId, result.AssignedHub, deviceCertificate))
                            return ResultCommand.CreateFailedCommand($"Error while testing the device connection.");

                        //Persist provision in TPM/HSM
                        SimulatedDevice.ProvisionDevice(result.AssignedHub, result.DeviceId);
                    }
                }
                if (deviceCertificate != null)
                    deviceCertificate.Dispose();

                return ResultCommand.CreateSuccessCommand();
            }
            catch (Exception e)
            {
                DebugHelper.LogCritical(e.Message);
                return ResultCommand.CreateFailedCommand(e.Message);
            }
        }

        private (X509Certificate2 deviceCertificate, X509Certificate2Collection collectionCertificates) LoadCertificateFromPfx(byte[] certificateRaw, string password)
        {
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(
                certificateRaw,
                password,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet);

            X509Certificate2 certificate = null;
            var outcollection = new X509Certificate2Collection();
            foreach (X509Certificate2 element in certificateCollection)
            {
                DebugHelper.LogVerbose($"Found certificate: {element?.Thumbprint} " +
                    $"{element?.Subject}; PrivateKey: {element?.HasPrivateKey}");
                if (certificate == null && element.HasPrivateKey)
                {
                    certificate = element;
                }
                else if (certificate == null && !((X509BasicConstraintsExtension)element.Extensions["Basic Constraints"]).CertificateAuthority)
                {
                    using (RSA key = SimulatedDevice.GetPrivateKey())
                    {
                        certificate = new X509Certificate2(RSACertificateExtensions.CopyWithPrivateKey(element, SimulatedDevice.GetPrivateKey())
                            .Export(X509ContentType.Pkcs12, SharedConstants.CERTIFICATE_PASSKEY), SharedConstants.CERTIFICATE_PASSKEY, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet);
                    }
                }
                else
                {
                    outcollection.Add(element);
                }
            }

            if (certificate == null)
            {
                DebugHelper.LogError($"ERROR: the certificate did not " +
                    $"contain any certificate with a private key.");
                return (null, null);
            }
            else
            {
                DebugHelper.LogVerbose($"Using certificate {certificate.Thumbprint} " +
                    $"{certificate.Subject}");
                return (certificate, outcollection);
            }
        }

        private async Task<bool> SaveCertificateInStore(X509Certificate2 deviceCertificate)
        {
            try
            {
                //Removing old certificates in the user certificate store. This requires the special capability "Shared User Certificates"
                await EnsureDeleteOldCertificates();

                //Adding certificate in user shared store, with no consent needed as other apps need to access it
                await CertificateEnrollmentManager.UserCertificateEnrollmentManager
                    .ImportPfxDataAsync(Convert.ToBase64String(deviceCertificate.Export(X509ContentType.Pkcs12, SharedConstants.CERTIFICATE_PASSKEY)), 
                    SharedConstants.CERTIFICATE_PASSKEY, ExportOption.Exportable, KeyProtectionLevel.NoConsent, InstallOptions.DeleteExpired, SimulatedDevice.DeviceId);

                return true;
            }
            catch(Exception e)
            {
                DebugHelper.LogCritical($"SaveCertificateInStore: {e.Message}");
                return false;
            }
        }

        private async Task EnsureDeleteOldCertificates()
        {
            UserCertificateStore certificateStore = CertificateStores.GetUserStoreByName(StandardCertificateStoreNames.Personal);
            var certificateFinder = await CertificateStores.FindAllAsync(new CertificateQuery() { IncludeDuplicates = true, IncludeExpiredCertificates = true });
            foreach (Certificate oldCertificate in certificateFinder)
            {
                if (oldCertificate.Issuer.StartsWith("Azure IoT Edison Intermediate"))
                {
                    try
                    {
                        await certificateStore.RequestDeleteAsync(oldCertificate);
                    }
                    catch (Exception e)
                    {
                        DebugHelper.LogWarning($"An old certificate could not be deleted: {e.Message}");
                    }
                }
            }
        }

        private async Task<bool> TestDeviceConnection(string deviceId, string hostname, X509Certificate2 deviceCertificate)
        {
            try
            {
                IAuthenticationMethod auth = new DeviceAuthenticationWithX509Certificate(deviceId, deviceCertificate);
                using (DeviceClient iotClient = DeviceClient.Create(hostname, auth, TransportType.Mqtt))
                {
                    DebugHelper.LogInformation("DeviceClient OpenAsync.");
                    await iotClient.OpenAsync();
                    DebugHelper.LogInformation("DeviceClient CloseAsync.");
                    await iotClient.CloseAsync();
                }
                return true;
            }
            catch(Exception e)
            {
                DebugHelper.LogCritical($"TestDeviceConnection: {e.Message}");
                return false;
            }
        }
    }
}
