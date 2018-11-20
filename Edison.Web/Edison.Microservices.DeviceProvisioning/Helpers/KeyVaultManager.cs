using Edison.DeviceProvisioning.Config;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Logging;
using Edison.DeviceProvisioning.Models;
using Microsoft.Azure.KeyVault.Models;
using System.Security.Cryptography.X509Certificates;

namespace Edison.DeviceProvisioning.Helpers
{
    public class KeyVaultManager
    {
        private readonly DeviceProvisioningOptions _config;
        private readonly AzureAdOptions _azureConfig;
        private readonly KeyVaultClient _keyVault;
        private readonly ILogger<KeyVaultManager> _logger;
        private readonly Random _random = new Random();

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

        public async Task<DeviceSecretKeysModel> GetSecretForDevice(Guid deviceId)
        {
            var output = new DeviceSecretKeysModel();

            var ssidPassword = await GetSecret($"{deviceId}-ssid");
            var portalPassword = await GetSecret($"{deviceId}-portal");
            var encryptionKey = await GetSecret($"{deviceId}-cryptkey");

            output.AccessPointPassword = !string.IsNullOrEmpty(ssidPassword) ? ssidPassword : _config.DefaultSecrets.AccessPointPassword;
            output.PortalPassword = !string.IsNullOrEmpty(portalPassword) ? portalPassword : _config.DefaultSecrets.PortalPassword;
            output.EncryptionKey = !string.IsNullOrEmpty(encryptionKey) ? encryptionKey : _config.DefaultSecrets.EncryptionKey;

            return output;
        }

        public async Task<DeviceSecretKeysModel> SetSecretForDevice(Guid deviceId)
        {
            DeviceSecretKeysModel output = new DeviceSecretKeysModel();

            var ssidPassword = await SetSecret($"{deviceId}-ssid", GenerateSecret(10));
            var portalPassword = await SetSecret($"{deviceId}-portal", GenerateSecret(10));
            var encryptionKey = await SetSecret($"{deviceId}-cryptkey", GenerateSecret(64));

            output.AccessPointPassword = !string.IsNullOrEmpty(ssidPassword) ? ssidPassword : _config.DefaultSecrets.AccessPointPassword;
            output.PortalPassword = !string.IsNullOrEmpty(portalPassword) ? portalPassword : _config.DefaultSecrets.PortalPassword;
            output.EncryptionKey = !string.IsNullOrEmpty(encryptionKey) ? encryptionKey : _config.DefaultSecrets.EncryptionKey;

            return output;
        }

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

        
        public string GenerateSecret(int length)
        {
            const string allowed = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(allowed, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
