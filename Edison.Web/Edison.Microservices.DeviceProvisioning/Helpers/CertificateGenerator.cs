using Edison.DeviceProvisioning.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Edison.DeviceProvisioning.Helpers
{
    /*public class CertificateGenerator
    {
        private readonly DeviceProvisioningOptions _config;
        private readonly ILogger<CertificateGenerator> _logger;

        public CertificateGenerator(IOptions<DeviceProvisioningOptions> config, ILogger<CertificateGenerator> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        // https://github.com/rwatjen/AzureIoTDPSCertificates
        public byte[] GenerateNewCertificate(DeviceProvisioningCertificateEntry signingCertificateInfo, string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                deviceId = Guid.NewGuid().ToString();
            _logger.LogInformation($"Create new certificate for device '{deviceId}', device type '{signingCertificateInfo.DeviceType}'");
            try
            {
                using (X509Certificate2 signingCert = new X509Certificate2(signingCertificateInfo.Path, signingCertificateInfo.Password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable)) {
                    using (RSA alg = RSA.Create(2048))
                    {
                        CertificateRequest certRequest = new CertificateRequest(
                            $"CN={deviceId}", alg, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                        certRequest.CertificateExtensions.Add(
                                new X509KeyUsageExtension(
                                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                                    true));

                        certRequest.CertificateExtensions.Add(
                            new X509BasicConstraintsExtension(false, true, 0, true));

                        //Get issuer object key
                        byte[] issuerSubjectKey = null;
                        if (signingCert.Extensions["Subject Key Identifier"] != null)
                            issuerSubjectKey = signingCert.Extensions["Subject Key Identifier"].RawData;
                        else if (signingCert.Extensions["X509v3 Subject Key Identifier"] != null) //Linux
                            issuerSubjectKey = signingCert.Extensions["X509v3 Subject Key Identifier"].RawData;
                        else
                            throw new Exception("Subject Key Identifier not found!");
                        var segment = new ArraySegment<byte>(issuerSubjectKey, 2, issuerSubjectKey.Length - 2);
                        var authorityKeyIdentifer = new byte[segment.Count + 4];
                        // these bytes define the "KeyID" part of the AuthorityKeyIdentifer
                        authorityKeyIdentifer[0] = 0x30;
                        authorityKeyIdentifer[1] = 0x16;
                        authorityKeyIdentifer[2] = 0x80;
                        authorityKeyIdentifer[3] = 0x14;
                        segment.CopyTo(authorityKeyIdentifer, 4);
                        certRequest.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509Extension("2.5.29.35", authorityKeyIdentifer, false));

                        // DPS samples create certs with the device name as a SAN name 
                        // in addition to the subject name
                        var sanBuilder = new SubjectAlternativeNameBuilder();
                        sanBuilder.AddDnsName(deviceId);
                        var sanExtension = sanBuilder.Build();
                        certRequest.CertificateExtensions.Add(sanExtension);

                        // Enhanced key usages
                        certRequest.CertificateExtensions.Add(
                            new X509EnhancedKeyUsageExtension(
                                new OidCollection {
                    new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
                    new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
                                },
                                false));

                        certRequest.CertificateExtensions.Add(
                            new X509SubjectKeyIdentifierExtension(certRequest.PublicKey, false));

                        //Generate unique serial
                        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        var unixTime = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
                        var serial = BitConverter.GetBytes(unixTime);

                        //Create leaf certificate
                        using (X509Certificate2 cert = certRequest.Create(
                            signingCert,
                            DateTimeOffset.UtcNow,
                            DateTimeOffset.UtcNow.AddDays(_config.DeviceCertificateValidityDays),
                            serial))
                        {
                            return CertificateUtils.GeneratePfxCertificate(
                                _config.DeviceCertificatePassword,
                                cert.CopyWithPrivateKey(alg),
                                signingCert);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError($"Error while creating new certificate for device '{deviceId}', device type '{signingCertificateInfo.DeviceType}'");
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
            }

            return null;
        }
    }*/
}
