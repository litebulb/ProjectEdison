using System;
using System.Net;
using Edison.Core.Common.Models;
using Edison.DeviceProvisioning.Config;
using Edison.DeviceProvisioning.Helpers;
using Edison.DeviceProvisioning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.KeyVault;

namespace Edison.DeviceProvisionning.Controllers
{
    /*[Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
    [Route("Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly DeviceProvisioningOptions _config;

        public SecurityController(IOptions<DeviceProvisioningOptions> config)
        {
            _config = config.Value;
        }

        [HttpPost]
        [Produces(typeof(DeviceCertificateModel))]
        public IActionResult GenerateDeviceKeys([FromBody]string deviceId)
        {
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Utils.GetToken));
            var cert = kv.GetCertificateAsync();
            cert.Result.
            var sec = await kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }*/
}
