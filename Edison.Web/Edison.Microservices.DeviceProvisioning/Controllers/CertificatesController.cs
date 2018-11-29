using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Edison.Core.Common;
using Edison.DeviceProvisioning.Config;
using Edison.DeviceProvisioning.Helpers;
using Edison.DeviceProvisioning.Models;

namespace Edison.DeviceProvisionning.Controllers
{
    /// <summary>
    /// Controller to handle certificate generation for the onboarding app
    /// </summary>
    [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
    [Route("Certificates")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly CertificateCsrSignator _certificateCsrSignator;
        private readonly DeviceProvisioningOptions _config;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public CertificatesController(IOptions<DeviceProvisioningOptions> config, CertificateCsrSignator certificateCsrSignator)
        {
            _config = config.Value;
            _certificateCsrSignator = certificateCsrSignator;
        }

        /// <summary>
        /// Generate a new device based on a CSR certificate
        /// </summary>
        /// <param name="deviceCertificateRequest">DeviceCertificateRequestModel containing DeviceType and CSR</param>
        /// <returns>DeviceCertificateModel containing signed certificate</returns>
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
