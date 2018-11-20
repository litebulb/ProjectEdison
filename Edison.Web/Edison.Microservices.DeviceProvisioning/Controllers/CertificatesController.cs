using System;
using System.Net;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.DeviceProvisioning.Config;
using Edison.DeviceProvisioning.Helpers;
using Edison.DeviceProvisioning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Edison.DeviceProvisionning.Controllers
{
    [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
    [Route("Certificates")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly CertificateCsrSignator _certificateCsrSignator;
        //private readonly CertificateGenerator _certificateGenerator; //CSR generation is recommended
        private readonly DeviceProvisioningOptions _config;

        //public CertificatesController(IOptions<DeviceProvisioningOptions> config, CertificateGenerator certificateGenerator, CertificateCsrSignator certificateCsrSignator)
        public CertificatesController(IOptions<DeviceProvisioningOptions> config, CertificateCsrSignator certificateCsrSignator)
        {
            _config = config.Value;
            //_certificateGenerator = certificateGenerator;
            _certificateCsrSignator = certificateCsrSignator;
        }

        [HttpPost]
        [Produces(typeof(DeviceCertificateModel))]
        public async Task<IActionResult> GenerateNewDeviceCertificate([FromBody]DeviceCertificateRequestModel deviceCertificateRequest)
        {
            //Get Intermediate certificate properties
            var certificateProperties = _config.SigningCertificates?.Find(p => p.DeviceType.ToLower().StartsWith(deviceCertificateRequest.DeviceType.ToLower()));

            if (certificateProperties == null)
                throw new Exception($"No matching devicetype found for {deviceCertificateRequest.DeviceType}.");

            byte[] certificate = null;

            //Use a Certificate Signature Request generate from the Device. Generally safer as the private key does not travel. //Currently not working
            if (!string.IsNullOrEmpty(deviceCertificateRequest.Csr))
                certificate = await _certificateCsrSignator.SignCertificate(certificateProperties, Convert.FromBase64String(deviceCertificateRequest.Csr));
            //Generate the certificate from the endpoint.
            //else
            //    certificate = _certificateGenerator.GenerateNewCertificate(certificateProperties, deviceCertificateRequest.DeviceId);

            if(certificate != null)
            {
                return Ok(new DeviceCertificateModel()
                {
                     DpsIdScope = _config.DPSIdScope,
                     DpsInstance = _config.DPSInstance,
                     Certificate = Convert.ToBase64String(certificate),
                     DeviceType = certificateProperties.DeviceType
                });
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
