using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Edison.DeviceProvisioning.Config;
using Edison.DeviceProvisioning.Models;

namespace Edison.DeviceProvisioning.Helpers
{
    /// <summary>
    /// Class that manages Azure Key Vault
    /// </summary>
    public class KeyVaultManager
    {
        private readonly DeviceProvisioningOptions _config;
        private readonly AzureAdOptions _azureConfig;
        private readonly KeyVaultClient _keyVault;
        private readonly ILogger<KeyVaultManager> _logger;
        private readonly Random _random = new Random();

        /// <summary>
        /// DI Constructor
        /// </summary>
        public KeyVaultManager(IOptions<AzureAdOptions> azureConfig, IOptions<DeviceProvisioningOptions> config, ILogger<KeyVaultManager> logger)
        {
            _config = config.Value;
            _azureConfig = azureConfig.Value;
            _logger = logger;
            _keyVault = new KeyVaultClient(async (authority, resource, scope) =>
            {
                var adCredential = new ClientCredential(_azureConfig.ClientId, _azureConfig.ClientSecret);
                var authenticationContext = new AuthenticationContext(authority, null);
                var result = await authenticationContext.AcquireTokenAsync(resource, adCredential);
                return result?.AccessToken;
            });
        }

        /// <summary>
        /// Retrieve the set of security keys for a particular device
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <returns>DeviceSecretKeysModel</returns>
        public async Task<DeviceSecretKeysModel> GetSecretForDevice(Guid deviceId)
        {
            var output = new DeviceSecretKeysModel();

            var ssidName = await GetSecret($"{deviceId}-ssidn");
            var ssidPassword = await GetSecret($"{deviceId}-ssidp");
            var portalPassword = await GetSecret($"{deviceId}-portal");
            var encryptionKey = await GetSecret($"{deviceId}-cryptkey");

            output.SSIDName = !string.IsNullOrEmpty(ssidName) ? ssidName : _config.DefaultSecrets.SSIDName;
            output.SSIDPassword = !string.IsNullOrEmpty(ssidPassword) ? ssidPassword : _config.DefaultSecrets.SSIDPassword;
            output.PortalPassword = !string.IsNullOrEmpty(portalPassword) ? portalPassword : _config.DefaultSecrets.PortalPassword;
            output.EncryptionKey = !string.IsNullOrEmpty(encryptionKey) ? encryptionKey : _config.DefaultSecrets.EncryptionKey;

            return output;
        }

        /// <summary>
        /// Generate a new set of keys for a particular device, and store it to Azure Key Vault.
        /// The Previous keys will be erased.
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>DeviceSecretKeysModel</returns>
        public async Task<DeviceSecretKeysModel> SetSecretForDevice(DeviceSecretKeysCreationModel deviceSecretKeysCreationRequest)
        {
            DeviceSecretKeysModel output = new DeviceSecretKeysModel();

            Guid deviceId = deviceSecretKeysCreationRequest.DeviceId;

            var ssidName = await SetSecret($"{deviceId}-ssidn", deviceSecretKeysCreationRequest.SSIDName);
            var ssidPassword = await SetSecret($"{deviceId}-ssidp", GenerateSecret(10));
            var portalPassword = await SetSecret($"{deviceId}-portal", GenerateSecret(10));
            var encryptionKey = await SetSecret($"{deviceId}-cryptkey", GenerateSecret(64));

            output.SSIDName = !string.IsNullOrEmpty(ssidName) ? ssidName : _config.DefaultSecrets.SSIDName;
            output.SSIDPassword = !string.IsNullOrEmpty(ssidPassword) ? ssidPassword : _config.DefaultSecrets.SSIDPassword;
            output.PortalPassword = !string.IsNullOrEmpty(portalPassword) ? portalPassword : _config.DefaultSecrets.PortalPassword;
            output.EncryptionKey = !string.IsNullOrEmpty(encryptionKey) ? encryptionKey : _config.DefaultSecrets.EncryptionKey;

            return output;
        }

        /// <summary>
        /// Get a CA Certificate from Azure Key Vault
        /// </summary>
        /// <param name="certificateIdentifier">CA X509Certificate2</param>
        /// <returns>X509Certificate2</returns>
        public async Task<X509Certificate2> GetCertificateAsync(string certificateIdentifier)
        {
            var secret = await _keyVault.GetSecretAsync(_config.KeyVaultAddress, certificateIdentifier);
            var bytes = Convert.FromBase64String(secret.Value);
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(bytes, null, X509KeyStorageFlags.Exportable);
            foreach(var certificate in certificateCollection)
                if (certificate.HasPrivateKey)
                    return certificate;
            return null;
        }

        /// <summary>
        /// Get a secret from its id
        /// </summary>
        /// <param name="secretIdentifier">Id of the secret</param>
        /// <returns>Value of the secret</returns>
        private async Task<string> GetSecret(string secretIdentifier)
        {
            try
            {
                var result = await _keyVault.GetSecretAsync(_config.KeyVaultAddress, secretIdentifier);
                return result.Value;
            }
            catch(KeyVaultErrorException e)
            {
                if (e.Body.Error.Code == "SecretNotFound")
                    return string.Empty;
                throw new Exception(string.Empty, e);
            }
            catch(Exception e)
            {
                _logger.LogError($"GetSecret: {e.Message}");
            }
            return string.Empty;
        }

        /// <summary>
        /// Set a secret
        /// </summary>
        /// <param name="secretIdentifier">Id of the secret</param>
        /// <param name="value">Value of the secret</param>
        /// <returns>Value of the secret</returns>
        private async Task<string> SetSecret(string secretIdentifier, string value)
        {
            try
            {
                var result = await _keyVault.SetSecretAsync(_config.KeyVaultAddress, secretIdentifier, value);
                return result.Value;
            }
            catch (Exception e)
            {
                _logger.LogError($"SetSecret: {e.Message}");
            }
            return string.Empty;
        }

        /// <summary>
        /// Generate a new secret
        /// </summary>
        /// <param name="length">Length of the secret</param>
        /// <returns>Value of the secret</returns>
        private string GenerateSecret(int length)
        {
            const string allowed = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(allowed, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
