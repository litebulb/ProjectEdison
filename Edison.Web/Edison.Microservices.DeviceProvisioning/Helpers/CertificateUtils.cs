using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Edison.DeviceProvisioning.Helpers
{
    public class CertificateUtils
    {
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

        private static X509Certificate2 ExportCertificatePublicKey(X509Certificate2 certificate)
        {
            var publicKeyBytes = certificate.Export(X509ContentType.Cert);
            var signingCertWithoutPrivateKey = new X509Certificate2(publicKeyBytes);
            return signingCertWithoutPrivateKey;
        }
    }
}
