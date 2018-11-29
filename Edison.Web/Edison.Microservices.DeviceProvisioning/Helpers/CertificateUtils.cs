using System.Security.Cryptography.X509Certificates;

namespace Edison.DeviceProvisioning.Helpers
{
    /// <summary>
    /// Set of utility methods for certificate generation
    /// </summary>
    public class CertificateUtils
    {
        /// <summary>
        /// Generate a PFX certificate
        /// </summary>
        /// <param name="password">Password for the certificate</param>
        /// <param name="certificate">Certificate</param>
        /// <param name="signingCert">CA Certificate</param>
        /// <param name="chain">Certificate chain</param>
        /// <returns>PFX Certificate Data</returns>
        public static byte[] GeneratePfxCertificate(string password,
            X509Certificate2 certificate, X509Certificate2 signingCert,
            X509Certificate2Collection chain = null)
        {
            var certCollection = new X509Certificate2Collection(certificate);
            if (chain != null)
            {
                certCollection.AddRange(chain);
            }
            if (signingCert != null)
            {
                var signingCertWithoutPrivateKey = ExportCertificatePublicKey(signingCert);
                certCollection.Add(signingCertWithoutPrivateKey);

            }
            return certCollection.Export(X509ContentType.Pfx, password);
        }

        /// <summary>
        /// Remove a private key from a X509Certificate2
        /// </summary>
        /// <param name="certificate">X509Certificate2 with private key</param>
        /// <returns>X509Certificate2 without private key</returns>
        private static X509Certificate2 ExportCertificatePublicKey(X509Certificate2 certificate)
        {
            var publicKeyBytes = certificate.Export(X509ContentType.Cert);
            var signingCertWithoutPrivateKey = new X509Certificate2(publicKeyBytes);
            return signingCertWithoutPrivateKey;
        }
    }
}
