using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Edison.DeviceProvisioning.Config;

namespace Edison.DeviceProvisioning.Helpers
{
    /// <summary>
    /// Class that relies on BouncyCastle to sign a certificate based on a CSR
    /// </summary>
    public class CertificateCsrSignator
    {
        private readonly DeviceProvisioningOptions _config;
        private readonly KeyVaultManager _keyVaultManager;
        private readonly ILogger<CertificateCsrSignator> _logger;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CertificateCsrSignator(IOptions<DeviceProvisioningOptions> config, KeyVaultManager keyVaultManager, ILogger<CertificateCsrSignator> logger)
        {
            _config = config.Value;
            _keyVaultManager = keyVaultManager;
            _logger = logger;
        }

        /// <summary>
        /// Sign a certificate
        /// </summary>
        /// <param name="signingCertificateInfo">DeviceProvisioningCertificateEntry</param>
        /// <param name="csrCert">CSR Data</param>
        /// <returns>PFX Certificate Data</returns>
        public async Task<byte[]> SignCertificate(DeviceProvisioningCertificateEntry signingCertificateInfo, byte[] csrCert)
        {
            //Get intermediate certificate from Azure key vault
            X509Certificate2 signingCert = await _keyVaultManager.GetCertificateAsync(signingCertificateInfo.CertificateIdentifier);

            //Sign the certificate
            X509Certificate2 signedCert = SignCertificate(signingCert, csrCert);

            //Return the certificate as a PFX.
            return CertificateUtils.GeneratePfxCertificate(_config.DeviceCertificatePassword,
                                signedCert, signingCert);
        }

        /// <summary>
        /// Sign a certificate
        /// </summary>
        /// <param name="signingCert">Issuer X509Certificate2</param>
        /// <param name="csrData">CSR Data</param>
        /// <returns>X509Certificate2 Certificate</returns>
        private X509Certificate2 SignCertificate(X509Certificate2 signingCert, byte[] csrData)
        {
            //Get CSR and retrieve public key
            Pkcs10CertificationRequest certRequest = new Pkcs10CertificationRequest(csrData);
            CertificationRequestInfo certInfo = certRequest.GetCertificationRequestInfo();
            SubjectPublicKeyInfo certPublicKeyInfo = certInfo.SubjectPublicKeyInfo;
            AsymmetricKeyParameter certPublicKey = PublicKeyFactory.CreateKey(certPublicKeyInfo);

            if (!certRequest.Verify(certPublicKey))
                throw new ApplicationException("The CSR is not valid: verification failed");

            //Get private key of intermediate issuer, to be used to sign the certificate
            var issuer = DotNetUtilities.FromX509Certificate(signingCert);
            AsymmetricCipherKeyPair keyPrivate = DotNetUtilities.GetRsaKeyPair(signingCert.GetRSAPrivateKey());
            //Get the serial number of the issuer
            var issuerSerialNumber = new BigInteger(signingCert.GetSerialNumber());

            //Generate a signed certificate
            return GenerateCertificate(certInfo.Subject, certPublicKey, certPublicKeyInfo,
                issuer.SubjectDN, issuer.GetPublicKey(), keyPrivate.Private, issuerSerialNumber);
        }

        /// <summary>
        /// Generate the certificate
        /// </summary>
        /// <param name="certName">Certificate name</param>
        /// <param name="certPublicKey">Certificate public key</param>
        /// <param name="certPublicKeyInfo">Certificate public key info</param>
        /// <param name="issuer">Issuer name</param>
        /// <param name="issuerPublicKey">Issuer public key</param>
        /// <param name="issuerPrivateKey">Issuer private key</param>
        /// <param name="issuerSerialNumber">Issuer serial number</param>
        /// <returns>X509Certificate2 Certificate</returns>
        private X509Certificate2 GenerateCertificate(X509Name certName, AsymmetricKeyParameter certPublicKey, SubjectPublicKeyInfo certPublicKeyInfo,
            X509Name issuer, AsymmetricKeyParameter issuerPublicKey, AsymmetricKeyParameter issuerPrivateKey, BigInteger issuerSerialNumber)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            ISignatureFactory factory = new Asn1SignatureFactory("SHA256withRSA", issuerPrivateKey, random);

            X509V3CertificateGenerator builder = new X509V3CertificateGenerator();
            builder.SetSerialNumber(new BigInteger(16, new SecureRandom(randomGenerator)));
            builder.SetIssuerDN(issuer);
            builder.SetSubjectDN(certName);
            builder.SetPublicKey(certPublicKey);
            builder.SetNotBefore(DateTime.UtcNow);
            builder.SetNotAfter(DateTime.UtcNow.AddDays(_config.DeviceCertificateValidityDays));

            builder.AddExtension(X509Extensions.KeyUsage.Id, true, new X509KeyUsage(X509KeyUsage.DigitalSignature | X509KeyUsage.KeyEncipherment));
            builder.AddExtension(X509Extensions.BasicConstraints.Id, true, new BasicConstraints(false));

            var subjectAlternativeNames = new Asn1Encodable[] { new GeneralName(GeneralName.DnsName, certName.GetValueList()[0].ToString()) };
            var subjectAlternativeNamesExtension = new DerSequence(subjectAlternativeNames);
            builder.AddExtension(X509Extensions.SubjectAlternativeName.Id, false, subjectAlternativeNamesExtension);

            builder.AddExtension(X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(new KeyPurposeID[]
            {
                KeyPurposeID.IdKPClientAuth,
                KeyPurposeID.IdKPServerAuth
            }
            ));

            builder.AddExtension(X509Extensions.AuthorityKeyIdentifier.Id, false, GetAuthorityKeyIdentifier(issuer, issuerPublicKey, issuerSerialNumber));
            builder.AddExtension(X509Extensions.SubjectKeyIdentifier.Id, false, new SubjectKeyIdentifier(certPublicKeyInfo));

            // Sign the certificate
            Org.BouncyCastle.X509.X509Certificate newCertificate = builder.Generate(factory);

            return new X509Certificate2(DotNetUtilities.ToX509Certificate(newCertificate));
        }

        /// <summary>
        /// Get the AuthorityKeyIdentifier Oid information from the issuer
        /// </summary>
        /// <param name="issuerDN">Issuer X509Name</param>
        /// <param name="publicKey">Issuer Public Key</param>
        /// <param name="issuerSerialNumber">Issuer Serial Number</param>
        /// <returns></returns>
        private AuthorityKeyIdentifier GetAuthorityKeyIdentifier(X509Name issuerDN, AsymmetricKeyParameter publicKey, BigInteger issuerSerialNumber)
        {
            return
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey),
                    new GeneralNames(new GeneralName(issuerDN)),
                    issuerSerialNumber);
        }
    }
}
